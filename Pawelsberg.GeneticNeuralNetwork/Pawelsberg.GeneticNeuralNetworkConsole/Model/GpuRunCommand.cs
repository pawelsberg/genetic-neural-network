using System.Diagnostics;
using Pawelsberg.GeneticNeuralNetwork.Gpu;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model;

public class GpuRunCommand : Command
{
    public static string Name = "gpurun";

    public override void Run(NetworkSimulation simulation)
    {
        TestCaseList tcl = simulation.TestCaseList;
        if (tcl == null || tcl.TestCases.Count == 0)
        {
            Console.WriteLine("No active test case list - use loadtcl <name> first.");
            return;
        }
        int propagations = simulation.Propagations;

        TestCase firstTestCase = tcl.TestCases[0];
        List<Network> seeds = simulation.CollectSeedSpecimens();
        if (seeds.Count == 0)
        {
            Console.WriteLine($"No seed networks in simulation; using CreateSimplest({firstTestCase.Inputs.Count}, {firstTestCase.Outputs.Count}).");
            seeds.Add(Network.CreateSimplest(firstTestCase.Inputs.Count, firstTestCase.Outputs.Count));
        }

        GpuLayout layout = ComputeLayout(seeds, tcl, simulation, propagations);

        Console.WriteLine($"Seeding GPU population from simulation: {seeds.Count} network(s) cycled across {layout.PopulationSize} slots.");
        Console.WriteLine($"Layout: MaxNodes={layout.MaxNodes} (mutator cap {layout.MutatorMaxNodes}), MaxSynapses={layout.MaxSynapses} (mutator cap {layout.MutatorMaxSynapses}), InputsPerNode={layout.MaxInputsPerNode}, OutputsPerNode={layout.MaxOutputsPerNode}, NetIn={layout.MaxNetworkInputs}, NetOut={layout.MaxNetworkOutputs}, TcIn={layout.MaxTestCaseInputs}, TcOut={layout.MaxTestCaseOutputs}");
        Console.WriteLine($"Running on GPU: {layout.PopulationSize} specimens x {GpuConstants.Generations} generations, {propagations} propagations");
        Console.WriteLine($"Test cases: {tcl.TestCases.Count}");

        Stopwatch sw = Stopwatch.StartNew();
        using GpuRunner runner = new GpuRunner(seeds, tcl, layout);
        Console.WriteLine($"GPU device: {runner.DeviceInfo}");
        Console.WriteLine($"Setup: {sw.ElapsedMilliseconds} ms. Starting evolution...");

        sw.Restart();
        Network best = runner.Run(GpuConstants.Generations, progressInterval: 100, (gen, fit) =>
        {
            Console.WriteLine($"  gen {gen,5}/{GpuConstants.Generations}  bestFitness={fit:F6}");
        });
        sw.Stop();

        float bestFitness = runner.ReadBestFitness();
        Console.WriteLine();
        Console.WriteLine($"Done in {sw.Elapsed.TotalSeconds:F2} s. Best fitness: {bestFitness:F6}");
        Console.WriteLine();
        Console.WriteLine("Best network:");
        Console.WriteLine(best.ToString());

        Console.WriteLine();
        Console.WriteLine("Alignment check on GPU's evolved best (matching-shape network):");
        PrintAlignmentCheck(runner, best, tcl, propagations);

        Network? mismatchSeed = seeds.FirstOrDefault(n =>
            n.Inputs.Count != firstTestCase.Inputs.Count || n.Outputs.Count != firstTestCase.Outputs.Count);
        if (mismatchSeed != null)
        {
            Console.WriteLine();
            Console.WriteLine($"Alignment check on a mismatched-shape seed (network has {mismatchSeed.Inputs.Count} inputs / {mismatchSeed.Outputs.Count} outputs - exercises penalty path):");
            PrintAlignmentCheck(runner, mismatchSeed, tcl, propagations);
        }
        else
        {
            Console.WriteLine();
            Console.WriteLine("(No mismatched-shape seed available; penalty path not tested here.)");
        }

        if (simulation.BestEver != null)
        {
            Console.WriteLine();
            Console.WriteLine($"BestEver staleness check (cached BestEverQuality recorded at last CPU tick may be from a different propagations / TestCaseList / meter setup):");
            Console.WriteLine($"  Cached BestEverQuality:  {simulation.BestEverQuality:F6}");
            QualityMeter<Network> cpuMeter = NetworkQualityMeters.CreateNormal(propagations, tcl);
            QualityMeasurement<Network> cpuNow = cpuMeter.MeasureQualityRecursive(simulation.BestEver, null);
            GpuEvaluation gpuNow = runner.Evaluate(simulation.BestEver);
            Console.WriteLine($"  CPU re-eval (current):   {cpuNow.Quality:F6}");
            Console.WriteLine($"  GPU eval (current):      {gpuNow.Fitness:F6}");
            Console.WriteLine($"  Cached - CPU current:    {simulation.BestEverQuality - cpuNow.Quality:F6}  (large value = stale cached quality)");
            Console.WriteLine($"  CPU current - GPU:       {cpuNow.Quality - gpuNow.Fitness:F6}  (large value = alignment gap)");
        }

        simulation.Add((Network)best.DeepClone());
    }

    private static GpuLayout ComputeLayout(IList<Network> seeds, TestCaseList tcl, NetworkSimulation simulation, int propagations)
    {
        int observedMaxNodes = 0;
        int observedMaxSynapses = 0;
        int observedMaxInputsPerNode = 0;
        int observedMaxOutputsPerNode = 0;
        int observedMaxNetIn = 0;
        int observedMaxNetOut = 0;
        foreach (Network n in seeds)
        {
            if (n.Nodes.Count > observedMaxNodes) observedMaxNodes = n.Nodes.Count;
            int synCount = n.SynapseCount();
            if (synCount > observedMaxSynapses) observedMaxSynapses = synCount;
            if (n.Inputs.Count > observedMaxNetIn) observedMaxNetIn = n.Inputs.Count;
            if (n.Outputs.Count > observedMaxNetOut) observedMaxNetOut = n.Outputs.Count;
            foreach (Node node in n.Nodes)
            {
                if (node.Outputs.Count > observedMaxOutputsPerNode) observedMaxOutputsPerNode = node.Outputs.Count;
                if (node is Neuron neuron && neuron.Inputs.Count > observedMaxInputsPerNode)
                    observedMaxInputsPerNode = neuron.Inputs.Count;
            }
        }

        int observedMaxTcIn = 0;
        int observedMaxTcOut = 0;
        foreach (TestCase tc in tcl.TestCases)
        {
            if (tc.Inputs.Count > observedMaxTcIn) observedMaxTcIn = tc.Inputs.Count;
            if (tc.Outputs.Count > observedMaxTcOut) observedMaxTcOut = tc.Outputs.Count;
        }

        // Buffer sizes accommodate observed networks AND any growth allowed by the
        // simulation's per-network caps. Mutators stop adding once the network reaches
        // the simulation's cap, but networks loaded from disk may already exceed it.
        int maxNodes = Math.Max(observedMaxNodes, simulation.MaxNodes);
        int maxSynapses = Math.Max(observedMaxSynapses, simulation.MaxSynapses);

        // Per-node fan-in/fan-out: max(observed, simulation.MaxSynapses). Mutator can
        // grow per-node connections only via SynapseAdder, which is itself capped at
        // simulation.MaxSynapses total synapses; any single neuron can therefore gain
        // at most simulation.MaxSynapses additional inputs/outputs through evolution
        // (and existing networks may already exceed that, which observed handles).
        int maxInputsPerNode = Math.Max(observedMaxInputsPerNode, simulation.MaxSynapses);
        int maxOutputsPerNode = Math.Max(observedMaxOutputsPerNode, simulation.MaxSynapses);

        // Network-level I/O can't grow under the simplified Normal mutator set.
        int maxNetIn = observedMaxNetIn;
        int maxNetOut = observedMaxNetOut;
        // CreateSimplest fallback might need at least 1 of each.
        if (maxNetIn == 0) maxNetIn = 1;
        if (maxNetOut == 0) maxNetOut = 1;

        return new GpuLayout(
            populationSize: GpuConstants.PopulationSize,
            maxNodes: maxNodes,
            maxSynapses: maxSynapses,
            maxInputsPerNode: maxInputsPerNode,
            maxOutputsPerNode: maxOutputsPerNode,
            maxNetworkInputs: maxNetIn,
            maxNetworkOutputs: maxNetOut,
            maxTestCaseInputs: Math.Max(observedMaxTcIn, 1),
            maxTestCaseOutputs: Math.Max(observedMaxTcOut, 1),
            mutatorMaxNodes: simulation.MaxNodes,
            mutatorMaxSynapses: simulation.MaxSynapses,
            propagations: propagations,
            testCaseCount: tcl.TestCases.Count);
    }

    private static void PrintAlignmentCheck(GpuRunner runner, Network network, TestCaseList tcl, int propagations)
    {
        GpuEvaluation gpuEval = runner.Evaluate(network);

        QualityMeter<Network> cpuMeter = NetworkQualityMeters.CreateNormal(propagations, tcl);
        QualityMeasurement<Network> cpuMeasurement = cpuMeter.MeasureQualityRecursive(network, null);
        double cpuQuality = cpuMeasurement.Quality;

        double[] cpuDiffs = ComputeCpuDiffs(network, tcl, propagations);

        Console.WriteLine($"  GPU fitness: {gpuEval.Fitness:F6}");
        Console.WriteLine($"  CPU quality: {cpuQuality:F6}");
        Console.WriteLine($"  Delta:       {(double)gpuEval.Fitness - cpuQuality:F6}");

        const double good = GpuConstants.GoodDifference;
        const double qDiff = GpuConstants.QualityForOneDiff;
        const double qGood = GpuConstants.QualityForGoodResult;
        const double qSub = GpuConstants.SubstractedQualityForOneDiff;
        // CPU sets diff = double.MaxValue on penalty; GPU sets diff = 1e30. Both are
        // symbolically infinite, so treat any diff above this threshold as "penalty"
        // when comparing - functionally they produce the same per-tc score.
        const double penaltyDiffThreshold = 1e29;

        int divergent = 0;
        double sumDiffDelta = 0d;
        double maxDelta = 0d;
        bool hasPenaltyRow = false;
        Console.WriteLine($"  Per-test-case (showing rows where score delta > 1e-4 or AllGood disagrees):");
        Console.WriteLine("    idx |       GPU diff |       CPU diff |  diff delta | GPU good | CPU good |  GPU score |  CPU score | score delta");
        for (int t = 0; t < tcl.TestCases.Count; t++)
        {
            double gpuDiff = gpuEval.PerTestCaseDiff[t];
            double cpuDiff = cpuDiffs[t];
            bool gpuPenalty = gpuDiff > penaltyDiffThreshold;
            bool cpuPenalty = cpuDiff > penaltyDiffThreshold;
            bool bothPenalty = gpuPenalty && cpuPenalty;
            if (bothPenalty) hasPenaltyRow = true;

            double diffDelta = bothPenalty ? 0d : Math.Abs(gpuDiff - cpuDiff);
            sumDiffDelta += diffDelta;
            if (diffDelta > maxDelta) maxDelta = diffDelta;

            bool gpuGood = gpuEval.PerTestCaseGood[t];
            bool cpuGood = cpuDiff < good;

            TestCase tc = tcl.TestCases[t];
            double cpuBase;
            if (network.Inputs.Count < tc.Inputs.Count || network.Outputs.Count < tc.Outputs.Count)
            {
                double inB = network.Inputs.Count < tc.Inputs.Count ? (double)network.Inputs.Count / (tc.Inputs.Count + 1) : 1.0;
                double outB = network.Outputs.Count < tc.Outputs.Count ? (double)network.Outputs.Count / (tc.Outputs.Count + 1) : 1.0;
                cpuBase = inB + outB;
            }
            else
                cpuBase = 2.0;

            double cpuScore = cpuBase
                + (cpuGood ? qDiff / good : qDiff / cpuDiff)
                + (cpuGood ? qGood : 0.0)
                + (cpuGood ? -qSub * cpuDiff : 0.0);

            double gpuScore = gpuEval.PerTestCaseScore[t];
            double scoreDelta = Math.Abs(gpuScore - cpuScore);

            if (scoreDelta > 1e-4 || gpuGood != cpuGood || gpuPenalty != cpuPenalty)
            {
                string gpuDiffStr = gpuPenalty ? "(penalty)" : gpuDiff.ToString("F6");
                string cpuDiffStr = cpuPenalty ? "(penalty)" : cpuDiff.ToString("F6");
                string deltaStr = bothPenalty ? "(both inf)" : diffDelta.ToString("G6");
                Console.WriteLine($"    {t,3} | {gpuDiffStr,14} | {cpuDiffStr,14} | {deltaStr,11} | {gpuGood,8} | {cpuGood,8} | {gpuScore,10:F4} | {cpuScore,10:F4} | {scoreDelta,11:G4}");
                divergent++;
            }
        }
        Console.WriteLine($"  Summary: {divergent} divergent rows; sum |diff delta|={sumDiffDelta:G6}, max |diff delta|={maxDelta:G6} (excluding both-penalty rows)");
        if (hasPenaltyRow)
            Console.WriteLine($"  Penalty path exercised (some tests have shape mismatch).");
        Console.WriteLine($"  GPU AllGood across all tcs = {gpuEval.PerTestCaseGood.All(g => g)}; CPU AllGood = {cpuDiffs.All(d => d < good)}");
    }

    private static double[] ComputeCpuDiffs(Network network, TestCaseList tcl, int propagations)
    {
        double[] diffs = new double[tcl.TestCases.Count];
        for (int t = 0; t < tcl.TestCases.Count; t++)
        {
            TestCase tc = tcl.TestCases[t];
            if (network.Inputs.Count < tc.Inputs.Count || network.Outputs.Count < tc.Outputs.Count)
            {
                diffs[t] = double.MaxValue;
                continue;
            }
            RunningContext ctx = network.SafeRun(tc, propagations);
            double diff = 0d;
            for (int o = 0; o < tc.Outputs.Count; o++)
                diff += Math.Abs(tc.Outputs[o] - ctx.GetPotential(network.Outputs[o]));
            diffs[t] = diff;
        }
        return diffs;
    }

    public override string ShortDescription { get { return "Run 1000 generations x 1000 specimens on GPU using simplified Normal mutators and Normal quality meter"; } }
}
