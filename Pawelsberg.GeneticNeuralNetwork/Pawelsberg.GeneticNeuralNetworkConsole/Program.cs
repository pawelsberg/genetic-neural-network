using System.Diagnostics;
using Pawelsberg.GeneticNeuralNetwork.Model;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics.Simulating;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting.DiskStoring;
using Pawelsberg.GeneticNeuralNetworkConsole.Model;

namespace Pawelsberg.GeneticNeuralNetworkConsole;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            MainImpl(args);
        }
        finally
        {
            GpuSimulationProvider.DisposeIfAny();
        }
    }

    static void MainImpl(string[] args)
    {
        NetworkSimulation simulation = new NetworkSimulation()
        //{
        //    TestCaseList = TestCaseLists.LoadTestCaseList("default")
        //}
        ;

        if (args.Length > 0 && args[0] == "--gpurun")
        {
            new InitCommand().Run(simulation);
            new LoadAllCommand().Run(simulation);
            if (args.Length > 1)
            {
                string tclName = args[1];
                Console.WriteLine($"Loading test case list: {tclName}");
                simulation.TestCaseList = TestCaseLists.LoadTestCaseList(tclName);
            }
            Console.WriteLine($"CPU BestEverQuality before gpurun: {simulation.BestEverQuality:F6} (BestEver: {(simulation.BestEver == null ? "null" : "(set)")})");
            GpuRunCommand cmd = new GpuRunCommand();
            if (args.Length > 2)
                cmd.LoadParameters(new CodedText(args[2]));
            cmd.Run(simulation);
            Console.WriteLine($"CPU BestEverQuality after gpurun: {simulation.BestEverQuality:F6} (BestEver Nodes:{simulation.BestEver?.Nodes.Count}, Synapses:{simulation.BestEver?.GetAllSynapses().Count()})");
            return;
        }

        // Smoke-test for gpustart / gpurun / gpupause as a sequence. Verifies the silent
        // reload-on-CPU-change path and the periodic CPU sync.
        // Usage: --gputest <tcl> [generations]
        if (args.Length > 0 && args[0] == "--gputest")
        {
            new InitCommand().Run(simulation);
            new LoadAllCommand().Run(simulation);
            if (args.Length > 1)
                simulation.TestCaseList = TestCaseLists.LoadTestCaseList(args[1]);
            string genArg = args.Length > 2 ? args[2] : "50";

            new GpuStartCommand().Run(simulation);
            Thread.Sleep(2000);
            new GpuPauseCommand().Run(simulation);

            // Exercise the silent reload hook: a load while a GPU sim exists should
            // tear it down and rebuild with the new seed list, preserving running state.
            Console.WriteLine("--- modifying CPU sim (load and) — expect silent GPU reload ---");
            LoadCommand load = new LoadCommand();
            load.LoadParameters(new CodedText("and"));
            load.Run(simulation);
            if (load.InvalidatesGpuSimulation)
                GpuSimulationProvider.Reload(simulation);

            Console.WriteLine($"CPU BestEverQuality before gpurun: {simulation.BestEverQuality:F6}");
            GpuRunCommand gpurun = new GpuRunCommand();
            gpurun.LoadParameters(new CodedText(genArg));
            gpurun.Run(simulation);
            Console.WriteLine($"CPU BestEverQuality after gpurun: {simulation.BestEverQuality:F6} (Nodes:{simulation.BestEver?.Nodes.Count}, Synapses:{simulation.BestEver?.GetAllSynapses().Count()})");
            return;
        }

        // Apples-to-apples comparison: bypass GpuSimulation (no worker thread, no
        // fence pacing, no locks) and run GpuRunner.Run synchronously on main thread.
        // This matches the original --gpurun behaviour; lets us see whether the worker
        // thread infra is the source of the CPU 12.5% / GPU 50% pattern, or whether the
        // shader pipeline itself is the ceiling.
        // Usage: --gpurun-sync <tcl> [generations]
        if (args.Length > 0 && args[0] == "--gpurun-sync")
        {
            new InitCommand().Run(simulation);
            new LoadAllCommand().Run(simulation);
            if (args.Length > 1)
                simulation.TestCaseList = TestCaseLists.LoadTestCaseList(args[1]);
            int gens = args.Length > 2 ? int.Parse(args[2]) : 1000;

            Network seed = simulation.BestEver
                ?? Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking.Network.CreateSimplest(simulation.TestCaseList.TestCases[0].Inputs.Count, simulation.TestCaseList.TestCases[0].Outputs.Count);
            Pawelsberg.GeneticNeuralNetwork.Gpu.GpuLayout layout = GpuSimulationProvider.ComputeLayout(seed, simulation.TestCaseList, simulation, simulation.Propagations);

            Stopwatch swSync = Stopwatch.StartNew();
            using Pawelsberg.GeneticNeuralNetwork.Gpu.GpuRunner runner = new Pawelsberg.GeneticNeuralNetwork.Gpu.GpuRunner(seed, simulation.TestCaseList, layout);
            Console.WriteLine($"Sync mode (no worker, no fences). Device: {runner.DeviceInfo}");
            Console.WriteLine($"Setup: {swSync.ElapsedMilliseconds} ms. Running {gens} gens synchronously...");
            swSync.Restart();
            runner.Run(gens, progressInterval: 100, (g, f) => Console.WriteLine($"  gen {g,5}/{gens} fit={f:F6}"));
            swSync.Stop();
            Console.WriteLine($"Sync done in {swSync.Elapsed.TotalSeconds:F2} s ({gens / swSync.Elapsed.TotalSeconds:F1} gens/sec).");
            return;
        }

        // Verifies that Ctrl+C / cancellation interrupts gpurun within seconds, not
        // minutes. Without the in-flight-fence cap the GPU command queue grows
        // unbounded and Pause + SyncNow waits for the entire backlog to drain.
        // Usage: --gpucanceltest <tcl> [waitBeforeCancelMs]
        if (args.Length > 0 && args[0] == "--gpucanceltest")
        {
            new InitCommand().Run(simulation);
            new LoadAllCommand().Run(simulation);
            if (args.Length > 1)
                simulation.TestCaseList = TestCaseLists.LoadTestCaseList(args[1]);
            int waitMs = args.Length > 2 ? int.Parse(args[2]) : 5000;

            // GpuSim construction creates the GLFW window — must happen on main thread.
            // Pre-create here so gpurun on the background thread reuses the existing sim.
            GpuSimulationProvider.GetOrCreate(simulation);

            // Run gpurun for a huge generation count on a background thread so we can
            // request cancellation from the main thread.
            GpuRunCommand gpurun = new GpuRunCommand();
            gpurun.LoadParameters(new CodedText("1000000"));
            Thread t = new Thread(() => gpurun.Run(simulation)) { IsBackground = true };
            Stopwatch totalSw = Stopwatch.StartNew();
            t.Start();
            Thread.Sleep(waitMs);
            Console.WriteLine($"--- requesting cancellation after {waitMs} ms ---");
            Stopwatch cancelSw = Stopwatch.StartNew();
            Cancellation.Request();
            t.Join();
            cancelSw.Stop();
            totalSw.Stop();
            Console.WriteLine($"Cancel-to-exit: {cancelSw.Elapsed.TotalSeconds:F2} s. Total: {totalSw.Elapsed.TotalSeconds:F2} s.");
            return;
        }

        // Reproduces the user's regression sequence:
        //   loadtcl <tcl> ; loadall ; set propagations <p> ; run <n> ; gpurun
        if (args.Length > 0 && args[0] == "--regression")
        {
            string tclName = args.Length > 1 ? args[1] : "mul3";
            int propagations = args.Length > 2 ? int.Parse(args[2]) : 20;
            int genCount = args.Length > 3 ? int.Parse(args[3]) : 1;

            new InitCommand().Run(simulation);
            simulation.TestCaseList = TestCaseLists.LoadTestCaseList(tclName);
            new LoadAllCommand().Run(simulation);
            simulation.Propagations = propagations;
            simulation.GenerationMultiplier = 1;

            Console.WriteLine($"--regression: tcl={tclName} propagations={propagations} cpuGens={genCount}");

            int startGen = simulation.GenerationNumber;
            int targetGen = startGen + genCount;
            EventHandler<NextGenerationCreatedEventArgs<Network>> handler = (sender, evtArgs) =>
            {
                if (simulation.GenerationNumber >= targetGen)
                    evtArgs.Pause = true;
            };
            simulation.NextGenerationCreated += handler;
            simulation.SimulationTimer.Start();
            while (simulation.SimulationTimer.Running)
                Thread.Sleep(20);
            simulation.NextGenerationCreated -= handler;

            Console.WriteLine($"After CPU run: GenerationNumber={simulation.GenerationNumber}, BestEverQuality={simulation.BestEverQuality:F6}, MaxPossibleQuality={simulation.MaxPossibleQuality:F6}");
            new GpuRunCommand().Run(simulation);
            return;
        }

        MainMenu mainMenu = new MainMenu(simulation);
        mainMenu.Run();
    }
}
