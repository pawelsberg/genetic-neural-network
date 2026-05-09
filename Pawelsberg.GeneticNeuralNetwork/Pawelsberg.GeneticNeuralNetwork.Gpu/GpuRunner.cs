using OpenTK.Graphics.OpenGL4;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;

namespace Pawelsberg.GeneticNeuralNetwork.Gpu;

public sealed class GpuRunner : IDisposable
{
    private GlContext? _context;
    private ComputeProgram? _propagate;
    private ComputeProgram? _score;
    private ComputeProgram? _aggregate;
    private ComputeProgram? _rank;
    private ComputeProgram? _updateBestEver;
    private ComputeProgram? _mutate;

    private GpuBuffer? _genomeI;
    private GpuBuffer? _genomeM;
    private GpuBuffer? _nextGenomeI;
    private GpuBuffer? _nextGenomeM;
    private GpuBuffer? _potentials;
    private GpuBuffer? _outputDiffs;
    private GpuBuffer? _perTestScores;
    private GpuBuffer? _perTestAllGood;
    private GpuBuffer? _fitness;
    private GpuBuffer? _specRank;
    private GpuBuffer? _parentByRank;
    private GpuBuffer? _testInputs;
    private GpuBuffer? _testOutputs;
    private GpuBuffer? _testInputCount;
    private GpuBuffer? _testOutputCount;
    private GpuBuffer? _rngStates;
    private GpuBuffer? _bestEverI;
    private GpuBuffer? _bestEverM;
    private GpuBuffer? _bestEverScalars;

    private bool _disposed;
    private bool _bootstrapped;
    private bool _genomeIsCurrent = true;
    private bool _ownsContext;

    private readonly GpuLayout _layout;

    public string DeviceInfo => _context!.GetDeviceInfo();
    public GpuLayout Layout => _layout;

    public GpuRunner(IList<Network> seeds, TestCaseList testCaseList, GpuLayout layout)
        : this(seeds, testCaseList, layout, externalContext: null) { }

    /// <summary>
    /// <paramref name="externalContext"/>: pass an already-created GlContext that is current
    /// on the calling thread (typical for off-main-thread GpuSimulation use, where the main
    /// thread builds the context and the worker MakeCurrents it). Pass null to let the
    /// constructor create a context bound to the calling thread (synchronous --gpurun path).
    /// The runner only disposes the context if it created it.
    /// </summary>
    public GpuRunner(IList<Network> seeds, TestCaseList testCaseList, GpuLayout layout, GlContext? externalContext)
    {
        if (seeds == null || seeds.Count == 0)
            throw new ArgumentException("At least one seed network is required", nameof(seeds));
        if (testCaseList.TestCases.Count != layout.TestCaseCount)
            throw new ArgumentException($"Layout TestCaseCount={layout.TestCaseCount} but testCaseList has {testCaseList.TestCases.Count}");
        _layout = layout;

        // If any allocation/compile fails partway through, release whatever was already
        // created (GL context, programs, buffers) before propagating the exception. The
        // caller never gets a reference back, so without this any partial state would leak
        // until process exit.
        try
        {
            if (externalContext == null)
            {
                _context = new GlContext();
                _ownsContext = true;
            }
            else
            {
                _context = externalContext;
                _ownsContext = false;
            }

            ShaderBuilder builder = new ShaderBuilder(layout);
            _propagate = new ComputeProgram(builder.BuildKernel("propagate.comp", false), "propagate");
            _score = new ComputeProgram(builder.BuildKernel("score_per_test_case.comp", false), "score");
            _aggregate = new ComputeProgram(builder.BuildKernel("aggregate_fitness.comp", false), "aggregate");
            _rank = new ComputeProgram(builder.BuildKernel("rank.comp", false), "rank");
            _updateBestEver = new ComputeProgram(builder.BuildKernel("update_best_ever.comp", false), "update_best_ever");
            _mutate = new ComputeProgram(builder.BuildKernel("mutate.comp", true), "mutate");

            int popSize = layout.PopulationSize;
            int genomeIBytes = popSize * layout.IntStridePerSpecimen * sizeof(int);
            int genomeMBytes = popSize * layout.MultiplierStridePerSpecimen * sizeof(double);
            _genomeI = new GpuBuffer("genomeI", genomeIBytes);
            _genomeM = new GpuBuffer("genomeM", genomeMBytes);
            _nextGenomeI = new GpuBuffer("nextGenomeI", genomeIBytes);
            _nextGenomeM = new GpuBuffer("nextGenomeM", genomeMBytes);

            _potentials = new GpuBuffer("potentials", popSize * layout.TestCaseCount * layout.MaxSynapses * sizeof(double));
            _outputDiffs = new GpuBuffer("outputDiffs", popSize * layout.TestCaseCount * sizeof(float));
            _perTestScores = new GpuBuffer("perTestScores", popSize * layout.TestCaseCount * sizeof(float));
            _perTestAllGood = new GpuBuffer("perTestAllGood", popSize * layout.TestCaseCount * sizeof(int));

            _fitness = new GpuBuffer("fitness", popSize * sizeof(float));
            _specRank = new GpuBuffer("specRank", popSize * sizeof(int));
            _parentByRank = new GpuBuffer("parentByRank", popSize * sizeof(int));

            _testInputs = new GpuBuffer("testInputs", layout.TestCaseCount * layout.MaxTestCaseInputs * sizeof(float));
            _testOutputs = new GpuBuffer("testOutputs", layout.TestCaseCount * layout.MaxTestCaseOutputs * sizeof(float));
            _testInputCount = new GpuBuffer("testInputCount", layout.TestCaseCount * sizeof(int));
            _testOutputCount = new GpuBuffer("testOutputCount", layout.TestCaseCount * sizeof(int));
            _rngStates = new GpuBuffer("rngStates", popSize * sizeof(uint));

            _bestEverI = new GpuBuffer("bestEverI", layout.IntStridePerSpecimen * sizeof(int));
            _bestEverM = new GpuBuffer("bestEverM", layout.MultiplierStridePerSpecimen * sizeof(double));
            // 4 ints: [floatBitsToInt(fitness), nodeCount, synapseCount, valid]
            _bestEverScalars = new GpuBuffer("bestEverScalars", 4 * sizeof(int));

            UploadInitialPopulation(seeds);
            UploadTestCases(testCaseList);
            UploadRngStates();
            ResetBestEver();
        }
        catch
        {
            DisposeInternal();
            throw;
        }
    }

    private void UploadInitialPopulation(IList<Network> seeds)
    {
        FlatGenome[] encoded = new FlatGenome[seeds.Count];
        for (int s = 0; s < seeds.Count; s++)
            encoded[s] = FlatGenome.Encode(seeds[s], _layout);

        int popSize = _layout.PopulationSize;
        int intStride = _layout.IntStridePerSpecimen;
        int multStride = _layout.MultiplierStridePerSpecimen;

        int[] allInts = new int[popSize * intStride];
        double[] allMults = new double[popSize * multStride];
        for (int s = 0; s < popSize; s++)
        {
            FlatGenome g = encoded[s % encoded.Length];
            Array.Copy(g.Ints, 0, allInts, s * intStride, intStride);
            Array.Copy(g.Multipliers, 0, allMults, s * multStride, multStride);
        }
        _genomeI!.UploadInts(allInts);
        _genomeM!.UploadDoubles(allMults);
    }

    private void UploadTestCases(TestCaseList testCaseList)
    {
        int tcCount = _layout.TestCaseCount;
        int maxIn = _layout.MaxTestCaseInputs;
        int maxOut = _layout.MaxTestCaseOutputs;
        float[] inputs = new float[tcCount * maxIn];
        float[] outputs = new float[tcCount * maxOut];
        int[] inputCounts = new int[tcCount];
        int[] outputCounts = new int[tcCount];
        for (int t = 0; t < tcCount; t++)
        {
            TestCase tc = testCaseList.TestCases[t];
            inputCounts[t] = tc.Inputs.Count;
            outputCounts[t] = tc.Outputs.Count;
            for (int i = 0; i < tc.Inputs.Count; i++)
                inputs[t * maxIn + i] = (float)tc.Inputs[i];
            for (int i = 0; i < tc.Outputs.Count; i++)
                outputs[t * maxOut + i] = (float)tc.Outputs[i];
        }
        _testInputs!.UploadFloats(inputs);
        _testOutputs!.UploadFloats(outputs);
        _testInputCount!.UploadInts(inputCounts);
        _testOutputCount!.UploadInts(outputCounts);
    }

    private void ResetBestEver()
    {
        // valid=0 means update_best_ever.comp will adopt the first generation's
        // rank-0 specimen unconditionally. The genome and multiplier buffers can
        // stay zero-initialized; they're only read after at least one adoption.
        _bestEverScalars!.UploadInts(new int[4]);
    }

    private void UploadRngStates()
    {
        int popSize = _layout.PopulationSize;
        int[] seeds = new int[popSize];
        for (int s = 0; s < popSize; s++)
        {
            uint v = (uint)(s + 1) * 0x9E3779B9u;
            v ^= 0xDEADBEEFu;
            if (v == 0u) v = 0x12345678u;
            seeds[s] = unchecked((int)v);
        }
        _rngStates!.UploadInts(seeds);
    }

    /// <summary>
    /// Initial-population evaluation — must be called once before any StepGeneration call.
    /// Mirrors gen-0 of the original Run loop: bind buffers, evaluate the seeded population,
    /// adopt rank-0 into BestEver. After this, ReadBestFitness reflects the seeded population.
    /// </summary>
    public void Bootstrap()
    {
        ThrowIfDisposed();
        if (_bootstrapped)
            throw new InvalidOperationException("GpuRunner already bootstrapped");
        _bootstrapped = true;
        _genomeIsCurrent = true;

        int popSize = _layout.PopulationSize;
        int popGroups = (popSize + 63) / 64;
        int specGroups = (popSize + 15) / 16;
        int tcGroups = (_layout.TestCaseCount + 3) / 4;

        BindBuffers(_genomeIsCurrent);
        DispatchEvaluate(specGroups, tcGroups, popGroups, 0);
        DispatchUpdateBestEver(0);
    }

    /// <summary>
    /// Run one generation: mutate the previous generation's parents into the next-buffer,
    /// swap, then evaluate and update BestEver. <paramref name="gen"/> is the index of the
    /// new generation (1-based; pass 1 for the first call after Bootstrap).
    /// </summary>
    public void StepGeneration(int gen)
    {
        ThrowIfDisposed();
        if (!_bootstrapped)
            throw new InvalidOperationException("GpuRunner.Bootstrap() must be called before StepGeneration");
        if (gen < 1)
            throw new ArgumentOutOfRangeException(nameof(gen), "gen must be >= 1; gen 0 is handled by Bootstrap");

        int popSize = _layout.PopulationSize;
        int popGroups = (popSize + 63) / 64;
        int specGroups = (popSize + 15) / 16;
        int tcGroups = (_layout.TestCaseCount + 3) / 4;

        BindBuffers(_genomeIsCurrent);
        _mutate!.Use();
        GL.Uniform1(_mutate!.GetUniformLocation("generationIndex"), (uint)(gen - 1));
        _mutate!.Dispatch(popGroups);
        GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);

        _genomeIsCurrent = !_genomeIsCurrent;
        BindBuffers(_genomeIsCurrent);
        DispatchEvaluate(specGroups, tcGroups, popGroups, gen);
        DispatchUpdateBestEver(gen);
    }

    public Network DownloadBestEverNetwork()
    {
        ThrowIfDisposed();
        int[] specInts = _bestEverI!.DownloadInts(_layout.IntStridePerSpecimen);
        double[] specMults = _bestEverM!.DownloadDoubles(_layout.MultiplierStridePerSpecimen);
        return FlatGenome.Decode(specInts, specMults, _layout);
    }

    /// <summary>
    /// Convenience wrapper: bootstrap + N step generations + download BestEver. Resets
    /// BestEver at the start so the returned network reflects only this run.
    /// </summary>
    public Network Run(int generations, int progressInterval = 0, Action<int, float>? onProgress = null)
    {
        ThrowIfDisposed();
        ResetBestEver();
        _bootstrapped = false;
        Bootstrap();
        if (onProgress != null && progressInterval > 0)
            onProgress(1, ReadBestFitness());

        for (int gen = 1; gen <= generations; gen++)
        {
            StepGeneration(gen);
            if (onProgress != null && progressInterval > 0 && ((gen + 1) % progressInterval == 0 || gen == generations))
                onProgress(gen + 1, ReadBestFitness());
        }

        return DownloadBestEverNetwork();
    }

    public float ReadBestFitness()
    {
        ThrowIfDisposed();
        int[] scalars = _bestEverScalars!.DownloadInts(4);
        return BitConverter.Int32BitsToSingle(scalars[0]);
    }

    /// <summary>
    /// Evaluate a single network on the GPU pipeline (no evolution). Returns the GPU
    /// fitness plus per-test-case diff/score/allGood. Replaces the entire population
    /// buffer, so call only outside the evolution loop.
    /// </summary>
    public GpuEvaluation Evaluate(Network network)
    {
        ThrowIfDisposed();
        FlatGenome g = FlatGenome.Encode(network, _layout);
        int popSize = _layout.PopulationSize;
        int intStride = _layout.IntStridePerSpecimen;
        int multStride = _layout.MultiplierStridePerSpecimen;

        int[] allInts = new int[popSize * intStride];
        double[] allMults = new double[popSize * multStride];
        Array.Copy(g.Ints, 0, allInts, 0, intStride);
        Array.Copy(g.Multipliers, 0, allMults, 0, multStride);
        _genomeI!.UploadInts(allInts);
        _genomeM!.UploadDoubles(allMults);

        int popGroups = (popSize + 63) / 64;
        int specGroups = (popSize + 15) / 16;
        int tcGroups = (_layout.TestCaseCount + 3) / 4;

        BindBuffers(true);
        DispatchEvaluate(specGroups, tcGroups, popGroups, 0);

        float[] diffsAll = _outputDiffs!.DownloadFloats(_layout.TestCaseCount);
        float[] scoresAll = _perTestScores!.DownloadFloats(_layout.TestCaseCount);
        int[] allGoodAll = _perTestAllGood!.DownloadInts(_layout.TestCaseCount);
        float[] fitsAll = _fitness!.DownloadFloats(1);

        bool[] good = new bool[_layout.TestCaseCount];
        for (int t = 0; t < _layout.TestCaseCount; t++) good[t] = allGoodAll[t] != 0;

        return new GpuEvaluation
        {
            Fitness = fitsAll[0],
            PerTestCaseDiff = diffsAll,
            PerTestCaseScore = scoresAll,
            PerTestCaseGood = good
        };
    }

    private void DispatchUpdateBestEver(int gen)
    {
        _updateBestEver!.Use();
        GL.Uniform1(_updateBestEver!.GetUniformLocation("generationIndex"), (uint)gen);
        _updateBestEver!.Dispatch(1);
        GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);
    }

    private void DispatchEvaluate(int specGroups, int tcGroups, int popGroups, int gen)
    {
        _propagate!.Use();
        GL.Uniform1(_propagate!.GetUniformLocation("generationIndex"), (uint)gen);
        _propagate!.Dispatch(specGroups, tcGroups);
        GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);

        _score!.Use();
        GL.Uniform1(_score!.GetUniformLocation("generationIndex"), (uint)gen);
        _score!.Dispatch(specGroups, tcGroups);
        GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);

        _aggregate!.Use();
        GL.Uniform1(_aggregate!.GetUniformLocation("generationIndex"), (uint)gen);
        _aggregate!.Dispatch(popGroups);
        GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);

        _rank!.Use();
        GL.Uniform1(_rank!.GetUniformLocation("generationIndex"), (uint)gen);
        _rank!.Dispatch(popGroups);
        GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);
    }

    private void BindBuffers(bool genomeIsCurrent)
    {
        if (genomeIsCurrent)
        {
            _genomeI!.Bind(0); _genomeM!.Bind(1);
            _nextGenomeI!.Bind(2); _nextGenomeM!.Bind(3);
        }
        else
        {
            _nextGenomeI!.Bind(0); _nextGenomeM!.Bind(1);
            _genomeI!.Bind(2); _genomeM!.Bind(3);
        }
        _potentials!.Bind(4);
        _outputDiffs!.Bind(5);
        _perTestScores!.Bind(6);
        _perTestAllGood!.Bind(7);
        _fitness!.Bind(8);
        _specRank!.Bind(9);
        _parentByRank!.Bind(10);
        _testInputs!.Bind(11);
        _testOutputs!.Bind(12);
        _rngStates!.Bind(13);
        _testInputCount!.Bind(14);
        _testOutputCount!.Bind(15);
        _bestEverI!.Bind(16);
        _bestEverM!.Bind(17);
        _bestEverScalars!.Bind(18);
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(GpuRunner));
    }

    public void Dispose()
    {
        if (_disposed) return;
        DisposeInternal();
        _disposed = true;
    }

    // Releases any resources that have been successfully created. Used both by
    // Dispose() and by the constructor's catch block to clean up partial state on
    // a mid-construction throw. Each field is independently nullable, so order does
    // not matter and previously-disposed (or never-allocated) slots are skipped.
    private void DisposeInternal()
    {
        _propagate?.Dispose(); _propagate = null;
        _score?.Dispose(); _score = null;
        _aggregate?.Dispose(); _aggregate = null;
        _rank?.Dispose(); _rank = null;
        _updateBestEver?.Dispose(); _updateBestEver = null;
        _mutate?.Dispose(); _mutate = null;
        _genomeI?.Dispose(); _genomeI = null;
        _genomeM?.Dispose(); _genomeM = null;
        _nextGenomeI?.Dispose(); _nextGenomeI = null;
        _nextGenomeM?.Dispose(); _nextGenomeM = null;
        _potentials?.Dispose(); _potentials = null;
        _outputDiffs?.Dispose(); _outputDiffs = null;
        _perTestScores?.Dispose(); _perTestScores = null;
        _perTestAllGood?.Dispose(); _perTestAllGood = null;
        _fitness?.Dispose(); _fitness = null;
        _specRank?.Dispose(); _specRank = null;
        _parentByRank?.Dispose(); _parentByRank = null;
        _testInputs?.Dispose(); _testInputs = null;
        _testOutputs?.Dispose(); _testOutputs = null;
        _testInputCount?.Dispose(); _testInputCount = null;
        _testOutputCount?.Dispose(); _testOutputCount = null;
        _rngStates?.Dispose(); _rngStates = null;
        _bestEverI?.Dispose(); _bestEverI = null;
        _bestEverM?.Dispose(); _bestEverM = null;
        _bestEverScalars?.Dispose(); _bestEverScalars = null;
        if (_ownsContext)
            _context?.Dispose();
        _context = null;
    }
}
