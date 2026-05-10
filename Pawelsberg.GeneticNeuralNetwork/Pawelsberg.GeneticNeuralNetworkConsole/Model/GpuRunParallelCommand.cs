using System.Diagnostics;
using Pawelsberg.GeneticNeuralNetwork.Gpu;
using Pawelsberg.GeneticNeuralNetwork.Model;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model;

/// <summary>
/// Run several isolated GPU evolutions back-to-back from the current CPU BestEver.
/// Each "island" gets the same starting network but a different RNG salt, so each one
/// follows a different mutation trajectory; at the end every island's BestEver is
/// added back to the CPU sim as a candidate. Useful to escape local optima — picking
/// the best of N independent runs from a common ancestor often beats one long run.
/// </summary>
public class GpuRunParallelCommand : Command
{
    public static string Name = "gpurunparallel";
    private int _generations;
    private int _islands;

    public override void LoadParameters(CodedText text)
    {
        if (text.EOT)
            throw new Exception("Usage: gpurunparallel <generations> <islands>");
        _generations = text.ReadInt();
        if (_generations < 1)
            throw new Exception("generations must be >= 1");
        text.SkipWhiteCharacters();
        if (text.EOT)
            throw new Exception("Usage: gpurunparallel <generations> <islands>");
        _islands = text.ReadInt();
        if (_islands < 1)
            throw new Exception("islands must be >= 1");
        text.SkipWhiteCharacters();
        if (!text.EOT)
            throw new Exception("Usage: gpurunparallel <generations> <islands>");
    }

    public override void Run(NetworkSimulation simulation)
    {
        TestCaseList tcl = simulation.TestCaseList;
        if (tcl == null || tcl.TestCases.Count == 0)
            throw new Exception("No active test case list - use loadtcl <name> first.");

        // Snapshot once at start — every island runs from the same ancestor so the
        // results are directly comparable. Falls back to the simplest network if the
        // CPU has no BestEver yet.
        TestCase firstTc = tcl.TestCases[0];
        Network seed = simulation.BestEver
            ?? Network.CreateSimplest(firstTc.Inputs.Count, firstTc.Outputs.Count);

        // Yield the GPU to islands by pausing any persistent GpuSim. We resume it at
        // the end if it was running. Multiple GL contexts on the same GPU can coexist
        // but they serialize at the driver level, so it's cleaner to take exclusive use.
        GpuSimulation? persistent = GpuSimulationProvider.GetExisting();
        bool persistentWasRunning = persistent != null && persistent.IsRunning;
        if (persistent != null) persistent.Pause();

        Console.WriteLine($"Running {_islands} island(s) x {_generations} generations from current BestEver (Nodes:{seed.Nodes.Count}, Synapses:{seed.GetAllSynapses().Count()}). Ctrl+C to interrupt.");
        Cancellation.Reset();

        GpuLayout layout = GpuSimulationProvider.ComputeLayout(seed, tcl, simulation, simulation.Propagations);
        QualityMeter<Network> cpuMeter = simulation.GenerationMeter;

        List<IslandResult> results = new List<IslandResult>();
        Stopwatch sw = Stopwatch.StartNew();
        try
        {
            for (int i = 1; i <= _islands; i++)
            {
                if (Cancellation.Requested)
                {
                    Console.WriteLine($"Cancelled before island {i}/{_islands}.");
                    break;
                }

                Stopwatch islandSw = Stopwatch.StartNew();
                using GpuSimulation island = new GpuSimulation(seed, tcl, layout, rngSalt: i)
                {
                    MaxStepsInFlight = GpuSimulationProvider.MaxGpuStepsInFlight,
                };
                island.EnsureStarted();
                island.RunUntil(_generations);
                while (island.IsRunning && !Cancellation.Requested)
                    Thread.Sleep(50);
                if (Cancellation.Requested)
                    island.Pause();

                Network result = island.DownloadBestEverNetwork();
                int actualGen = island.GenerationNumber;
                float gpuFitness = island.BestEverFitness;
                double cpuQuality = cpuMeter != null
                    ? cpuMeter.MeasureQualityRecursive(result, null).Quality
                    : double.NaN;
                islandSw.Stop();

                results.Add(new IslandResult(i, result, gpuFitness, cpuQuality, actualGen, islandSw.Elapsed));
                Console.WriteLine($"  Island {i,2}/{_islands}: gen {actualGen}/{_generations} in {islandSw.Elapsed.TotalSeconds,6:F2}s — GPU fit={gpuFitness:F6}, CPU quality={cpuQuality:F6}, Nodes={result.Nodes.Count}, Synapses={result.GetAllSynapses().Count()}");
            }
        }
        finally
        {
            sw.Stop();
            // Resume the persistent GpuSim regardless of how the islands ended (success
            // or Ctrl+C) so we don't leave the user's earlier gpustart abandoned.
            if (persistentWasRunning && persistent != null)
                persistent.Start();
        }

        if (results.Count == 0)
        {
            Console.WriteLine("No island results to add.");
            return;
        }

        // Add all island results to the CPU sim. AddAndRefreshBestEver re-evaluates each
        // via the CPU quality meter and updates BestEver if any beats the current one,
        // so `show` immediately reflects the best of the bunch.
        foreach (IslandResult r in results)
            simulation.AddAndRefreshBestEver(r.Network);

        // Final tally: the island whose CPU-evaluated quality is highest. Lets the user
        // see at a glance which evolution path won (and how much spread there was).
        IslandResult best = results.OrderByDescending(r => r.CpuQuality).First();
        Console.WriteLine($"Done in {sw.Elapsed.TotalSeconds:F2}s. Best island: #{best.Index} (CPU quality {best.CpuQuality:F6}). CPU BestEverQuality is now {simulation.BestEverQuality:F6}.");
    }

    private sealed record IslandResult(int Index, Network Network, float GpuFitness, double CpuQuality, int Generation, TimeSpan Elapsed);

    public override string ShortDescription { get { return "Run N islands x G generations from current BestEver, each with independent RNG. Args: <generations> <islands>"; } }
}
