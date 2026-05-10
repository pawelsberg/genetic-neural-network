using OpenTK.Graphics.OpenGL4;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;

namespace Pawelsberg.GeneticNeuralNetwork.Gpu;

/// <summary>
/// Background-driven GPU evolution. Mirrors NetworkSimulation's start/pause semantics on
/// the CPU side: a dedicated worker thread owns the GL context and the GpuRunner, iterates
/// generations while Running, and parks otherwise. Cross-thread reads (e.g. downloading the
/// current BestEver network) are dispatched via a job queue and processed between
/// generations so all GL calls stay on the worker thread.
/// </summary>
public sealed class GpuSimulation : IDisposable
{
    // Single seed network — the CPU sim's BestEver. Avoids transferring the full CPU
    // generation across the CPU/GPU boundary; the GPU mutates 1000 copies of the seed
    // into its own population in a few generations anyway.
    private readonly Network _seed;
    private readonly TestCaseList _testCaseList;
    private readonly int _rngSalt;
    private readonly GpuLayout _layout;
    private readonly GlContext _glContext;

    private readonly object _lock = new object();
    private readonly Queue<Action<GpuRunner>> _jobs = new Queue<Action<GpuRunner>>();
    private readonly AutoResetEvent _wake = new AutoResetEvent(false);
    private readonly ManualResetEventSlim _initSignal = new ManualResetEventSlim(false);

    private Thread? _worker;
    private bool _stopRequested;
    private bool _running;
    private int _generationNumber;
    private int _targetGeneration = -1;
    private float _bestEverFitness;
    private int _lastSyncGeneration;
    private string _deviceInfo = "(uninitialized)";
    private Exception? _initException;
    private bool _disposed;
    private Action<Network, float>? _periodicSyncCallback;
    private int _periodicSyncInterval = 1000;
    private int _maxStepsInFlight = 64;
    // Worker-thread-only — never touched off-thread, so no lock needed.
    private readonly Queue<IntPtr> _inFlightFences = new Queue<IntPtr>();
    // We hold at most this many sync objects regardless of MaxStepsInFlight; the
    // fence-insertion cadence (stepsPerFence) is derived to give the desired step cap.
    // Rationale: per-step glFenceSync + glClientWaitSync drives serious driver-thread
    // work; spacing fences out by ~8 steps cuts that overhead ~8x while still bounding
    // the queue depth. Min 1 fence-per-step when MaxStepsInFlight is tiny.
    private const int FenceCount = 8;
    private int _stepsSinceLastFence;

    /// <summary>
    /// Must be constructed on the main thread: GLFW requires window creation there. The
    /// constructor creates the GlContext (binds it to the main thread) and immediately
    /// releases it so the worker thread can MakeCurrent on itself when EnsureStarted runs.
    /// </summary>
    public GpuSimulation(Network seed, TestCaseList testCaseList, GpuLayout layout, int rngSalt = 0)
    {
        _seed = seed;
        _testCaseList = testCaseList;
        _layout = layout;
        _rngSalt = rngSalt;
        _glContext = new GlContext();
        _glContext.ReleaseCurrent();
    }

    public string DeviceInfo { get { lock (_lock) return _deviceInfo; } }
    public int GenerationNumber => Volatile.Read(ref _generationNumber);
    /// <summary>Last known BestEver fitness. Refreshed on every periodic-sync tick and on
    /// every SyncNow() call; stale by up to PeriodicSyncInterval generations otherwise.
    /// Reading per generation would force a CPU/GPU sync and cap utilisation around 50%,
    /// so the worker does NOT download fitness on every step.</summary>
    public float BestEverFitness { get { lock (_lock) return _bestEverFitness; } }
    public bool IsRunning { get { lock (_lock) return _running; } }
    public bool IsAlive => _worker != null && _worker.IsAlive;
    public GpuLayout Layout => _layout;
    public TestCaseList TestCaseList => _testCaseList;

    /// <summary>Invoked from the worker thread every PeriodicSyncInterval generations and
    /// also from SyncNow() (which is what gpurun-end and gpupause use). Gets the freshly
    /// downloaded BestEver network and its fitness; the typical wiring is to forward the
    /// network into the CPU NetworkSimulation via simulation.Add(network).</summary>
    public Action<Network, float>? PeriodicSyncCallback
    {
        get { lock (_lock) return _periodicSyncCallback; }
        set { lock (_lock) _periodicSyncCallback = value; }
    }

    public int PeriodicSyncInterval
    {
        get { lock (_lock) return _periodicSyncInterval; }
        set { lock (_lock) _periodicSyncInterval = Math.Max(1, value); }
    }

    /// <summary>
    /// Cap on how many StepGeneration calls the worker is allowed to enqueue ahead of the
    /// GPU. Without a cap, GL dispatches are async and the worker outpaces the GPU by
    /// thousands of steps, making Pause/Ctrl+C wait minutes for the queue to drain. With
    /// fence-based pacing, the worker waits for the oldest fence before queuing the
    /// (N+1)th step, bounding cancel latency to ~N step times. Higher values give the
    /// GPU more pipelining headroom; lower values give snappier cancellation.
    /// </summary>
    public int MaxStepsInFlight
    {
        get { lock (_lock) return _maxStepsInFlight; }
        set { lock (_lock) _maxStepsInFlight = Math.Max(1, value); }
    }

    /// <summary>
    /// Spin up the worker thread (which constructs the GpuRunner so its GL context is
    /// bound to that thread) and wait until the initial population has been evaluated.
    /// Throws on init failure (e.g. shader link errors).
    /// </summary>
    public void EnsureStarted()
    {
        ThrowIfDisposed();
        bool spawn = false;
        lock (_lock)
        {
            if (_worker == null)
            {
                _worker = new Thread(WorkerLoop) { IsBackground = true, Name = "GpuSimulation" };
                spawn = true;
            }
        }
        if (spawn)
            _worker!.Start();
        _initSignal.Wait();
        if (_initException != null)
            throw new Exception("GPU simulation initialization failed", _initException);
    }

    public void Start()
    {
        EnsureStarted();
        lock (_lock) { _running = true; _targetGeneration = -1; }
        _wake.Set();
    }

    public void Pause()
    {
        lock (_lock) { _running = false; _targetGeneration = -1; }
        _wake.Set();
    }

    /// <summary>
    /// Run generations until <paramref name="targetGen"/> is reached, then auto-pause.
    /// Worker-enforced (not polling-loop enforced): StepGeneration is async — by the time
    /// a polling caller observes GenerationNumber crossing the target, the worker has
    /// usually enqueued many extra generations. Setting the target here makes the worker
    /// itself stop the moment it commits the targeted gen.
    /// </summary>
    public void RunUntil(int targetGen)
    {
        EnsureStarted();
        lock (_lock) { _running = true; _targetGeneration = targetGen; }
        _wake.Set();
    }

    /// <summary>
    /// Submit a download of the current BestEver network and block until the worker
    /// returns it. Safe to call while Running — the worker processes the job between
    /// two consecutive generation steps.
    /// </summary>
    public Network DownloadBestEverNetwork()
    {
        EnsureStarted();
        Network? result = null;
        Exception? error = null;
        ManualResetEventSlim done = new ManualResetEventSlim(false);
        EnqueueJob(r =>
        {
            try { result = r.DownloadBestEverNetwork(); }
            catch (Exception e) { error = e; }
            finally { done.Set(); }
        });
        done.Wait();
        if (error != null)
            throw new Exception("DownloadBestEverNetwork failed", error);
        return result!;
    }

    private void EnqueueJob(Action<GpuRunner> job)
    {
        lock (_lock) _jobs.Enqueue(job);
        _wake.Set();
    }

    /// <summary>
    /// Force a sync now: download BestEver + fitness on the worker thread and invoke the
    /// PeriodicSyncCallback (which typically pushes the network into the CPU sim). Used
    /// by gpurun (end of run) and gpupause so the CPU sim sees the freshest GPU result
    /// immediately rather than waiting for the next periodic tick. Blocks until done.
    /// </summary>
    public void SyncNow()
    {
        EnsureStarted();
        Action<Network, float>? cb;
        lock (_lock) cb = _periodicSyncCallback;
        if (cb == null) return;

        ManualResetEventSlim done = new ManualResetEventSlim(false);
        Exception? error = null;
        EnqueueJob(r =>
        {
            try { DoSyncCallback(r, cb); }
            catch (Exception e) { error = e; }
            finally { done.Set(); }
        });
        done.Wait();
        if (error != null)
            throw new Exception("SyncNow failed", error);
    }

    private void DoSyncCallback(GpuRunner runner, Action<Network, float> cb)
    {
        // The download forces a GPU sync, so all in-flight fences are guaranteed signalled
        // by the time it returns. Drain the queue first so we don't leak sync objects.
        DrainAllFences();
        Network best = runner.DownloadBestEverNetwork();
        float fit = runner.ReadBestFitness();
        lock (_lock) _bestEverFitness = fit;
        cb(best, fit);
    }

    // Worker-thread-only. Pops the oldest fence, waits for the GPU to signal it, and
    // releases the sync object. Uses a non-blocking poll + Thread.Sleep(1) instead of
    // ClientWaitSync(timeout=infinite) because NVIDIA's implementation busy-spins for
    // a noticeable interval before falling back to a kernel wait — which keeps the
    // worker thread at full single-core utilisation even though it's "waiting." The
    // explicit Thread.Sleep guarantees the OS parks the thread; latency cost is
    // ~Windows scheduler tick (~15 ms) per outstanding fence, negligible vs. the
    // step time on the workloads where this fires.
    private void WaitAndDeleteOldestFence()
    {
        IntPtr oldest = _inFlightFences.Dequeue();
        // Issue one flush before the polling loop so commands actually reach the GPU
        // (otherwise the fence may sit unsignaled forever — same role as
        // ClientWaitSync's SyncFlushCommandsBit, but only paid once).
        WaitSyncStatus status = GL.ClientWaitSync(oldest, ClientWaitSyncFlags.SyncFlushCommandsBit, 0);
        while (status != WaitSyncStatus.AlreadySignaled && status != WaitSyncStatus.ConditionSatisfied)
        {
            Thread.Sleep(1);
            status = GL.ClientWaitSync(oldest, 0, 0);
        }
        GL.DeleteSync(oldest);
    }

    private void DrainAllFences()
    {
        while (_inFlightFences.Count > 0)
            WaitAndDeleteOldestFence();
    }

    private void WorkerLoop()
    {
        GpuRunner? runner = null;
        try
        {
            // Take ownership of the context (main thread released it after construction).
            _glContext.MakeCurrent();
            runner = new GpuRunner(_seed, _testCaseList, _layout, _glContext, _rngSalt);
            lock (_lock) _deviceInfo = runner.DeviceInfo;
            runner.Bootstrap();
            lock (_lock) _bestEverFitness = runner.ReadBestFitness();
        }
        catch (Exception ex)
        {
            _initException = ex;
            runner?.Dispose();
            _initSignal.Set();
            return;
        }
        _initSignal.Set();

        while (true)
        {
            // Drain queued jobs first so reads from other threads don't wait for a
            // generation step. Job handlers run on this thread (the GL-context owner).
            while (true)
            {
                Action<GpuRunner>? job = null;
                lock (_lock)
                {
                    if (_jobs.Count > 0) job = _jobs.Dequeue();
                }
                if (job == null) break;
                try { job(runner); }
                catch { /* swallow — caller's TaskCompletionSource captures the error */ }
            }

            if (_stopRequested) break;

            bool running;
            lock (_lock) running = _running;

            if (running)
            {
                int gen = Volatile.Read(ref _generationNumber) + 1;
                runner.StepGeneration(gen);
                Volatile.Write(ref _generationNumber, gen);

                // Pace the worker: insert a fence every `stepsPerFence` steps and, when we
                // already hold FenceCount unfinished ones, wait for the oldest. This bounds
                // the GPU command queue depth (~MaxStepsInFlight steps), without per-step
                // fence overhead — `glFenceSync`/`glClientWaitSync` per step adds enough
                // CPU work to starve the GPU on small workloads. Spacing fences out keeps
                // GPU saturated; cancellation latency stays bounded by MaxStepsInFlight.
                int maxInFlight;
                lock (_lock) maxInFlight = _maxStepsInFlight;
                int stepsPerFence = Math.Max(1, maxInFlight / FenceCount);
                _stepsSinceLastFence++;
                if (_stepsSinceLastFence >= stepsPerFence)
                {
                    _stepsSinceLastFence = 0;
                    IntPtr fence = GL.FenceSync(SyncCondition.SyncGpuCommandsComplete, 0);
                    _inFlightFences.Enqueue(fence);
                    while (_inFlightFences.Count > FenceCount)
                        WaitAndDeleteOldestFence();
                }

                // Keep the GPU command queue full: do NOT download fitness per step (that
                // forces a CPU/GPU sync and tanks utilisation). Only sync to CPU at the
                // configured interval.
                int interval;
                Action<Network, float>? cb;
                int target;
                lock (_lock) { interval = _periodicSyncInterval; cb = _periodicSyncCallback; target = _targetGeneration; }
                if (cb != null && gen - _lastSyncGeneration >= interval)
                {
                    DoSyncCallback(runner, cb);
                    _lastSyncGeneration = gen;
                }
                if (target >= 0 && gen >= target)
                    lock (_lock) { _running = false; _targetGeneration = -1; }
            }
            else
            {
                // Park until a job is enqueued, Start/Pause is called, or Dispose runs.
                _wake.WaitOne(100);
            }
        }

        // Wait for any unfinished GPU work + release sync handles before we tear the
        // runner down — avoids leaking fences and orphaning queued dispatches.
        DrainAllFences();
        runner.Dispose();
        // Release the context from this thread so main can dispose the window
        // (glfwDestroyWindow must run on the main thread).
        _glContext.ReleaseCurrent();
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(GpuSimulation));
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _stopRequested = true;
        _wake.Set();
        if (_worker != null && _worker.IsAlive)
            _worker.Join();
        _wake.Dispose();
        _initSignal.Dispose();
        // Window destruction must happen on the main thread (GLFW limitation). Dispose
        // is the main-thread shutdown path (see Program.Main's finally), so this is safe.
        _glContext.Dispose();
    }
}
