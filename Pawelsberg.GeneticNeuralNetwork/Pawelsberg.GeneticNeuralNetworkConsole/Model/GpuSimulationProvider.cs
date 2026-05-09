using Pawelsberg.GeneticNeuralNetwork.Gpu;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model;

/// <summary>
/// Process-wide holder for the persistent GpuSimulation. The first gpu* command lazily
/// constructs the simulation from the current NetworkSimulation state (test cases, seeds,
/// propagations, MaxNodes/MaxSynapses). Subsequent gpu* commands reuse it so background
/// evolution can persist across commands. Caller-side reset is intentionally not exposed —
/// quitting the program tears it down via DisposeIfAny().
/// </summary>
public static class GpuSimulationProvider
{
    private static GpuSimulation? _instance;
    private static readonly object _lock = new object();

    /// <summary>
    /// Configurable GPU population size — `set maxGpuSpecimens N` in the console. Lives on
    /// the provider rather than on NetworkSimulation because the core library has no GPU
    /// dependency. Used at GpuLayout construction time, so a change only takes effect on
    /// the next gpu* command (which triggers a reload via the InvalidatesGpuSimulation hook).
    /// </summary>
    public static int MaxGpuSpecimens { get; set; } = GpuConstants.PopulationSize;

    public static GpuSimulation? GetExisting()
    {
        lock (_lock) return _instance;
    }

    public static GpuSimulation GetOrCreate(NetworkSimulation simulation)
    {
        lock (_lock)
        {
            if (_instance != null) return _instance;

            TestCaseList tcl = simulation.TestCaseList;
            if (tcl == null || tcl.TestCases.Count == 0)
                throw new Exception("No active test case list - use loadtcl <name> first.");

            TestCase firstTestCase = tcl.TestCases[0];
            List<Network> seeds = simulation.CollectSeedSpecimens();
            if (seeds.Count == 0)
            {
                Console.WriteLine($"No seed networks in simulation; using CreateSimplest({firstTestCase.Inputs.Count}, {firstTestCase.Outputs.Count}).");
                seeds.Add(Network.CreateSimplest(firstTestCase.Inputs.Count, firstTestCase.Outputs.Count));
            }

            GpuLayout layout = ComputeLayout(seeds, tcl, simulation, simulation.Propagations);
            GpuSimulation sim = new GpuSimulation(seeds, tcl, layout);
            // Wire the periodic CPU sync: every PeriodicSyncInterval generations (and on
            // every SyncNow call from gpurun-end / gpupause), push the GPU's BestEver into
            // the CPU NetworkSimulation. AddAndRefreshBestEver re-evaluates the candidate
            // against the CPU quality meter and updates BestEver synchronously, so a `show`
            // immediately after gpurun/gpupause reflects the GPU's freshly-evolved network
            // instead of the stale CPU BestEver. Clone so the CPU sim doesn't share the
            // GPU-decoded instance.
            sim.PeriodicSyncCallback = (network, fitness) =>
            {
                Network clone = (Network)network.DeepClone();
                simulation.AddAndRefreshBestEver(clone);
            };
            _instance = sim;
            return _instance;
        }
    }

    public static void DisposeIfAny()
    {
        lock (_lock)
        {
            _instance?.Dispose();
            _instance = null;
        }
    }

    /// <summary>
    /// Silently rebuild the GPU simulation from current CPU state. Preserves the
    /// running/paused state across the rebuild — if the user was mid-run when they loaded
    /// a network or changed a setting, the GPU keeps running with the new seeds + layout.
    /// No console output (the user's edit was the focus, the GPU rebuild is incidental).
    /// </summary>
    public static void Reload(NetworkSimulation simulation)
    {
        GpuSimulation? old;
        bool wasRunning;
        lock (_lock)
        {
            if (_instance == null) return;
            old = _instance;
            wasRunning = old.IsRunning;
            _instance = null;
        }
        old.Dispose();
        if (wasRunning)
        {
            GpuSimulation fresh = GetOrCreate(simulation);
            fresh.Start();
        }
    }

    public static GpuLayout ComputeLayout(IList<Network> seeds, TestCaseList tcl, NetworkSimulation simulation, int propagations)
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

        int maxNodes = Math.Max(observedMaxNodes, simulation.MaxNodes);
        int maxSynapses = Math.Max(observedMaxSynapses, simulation.MaxSynapses);
        int maxInputsPerNode = Math.Max(observedMaxInputsPerNode, simulation.MaxSynapses);
        int maxOutputsPerNode = Math.Max(observedMaxOutputsPerNode, simulation.MaxSynapses);
        int maxNetIn = observedMaxNetIn;
        int maxNetOut = observedMaxNetOut;
        if (maxNetIn == 0) maxNetIn = 1;
        if (maxNetOut == 0) maxNetOut = 1;

        return new GpuLayout(
            populationSize: MaxGpuSpecimens,
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
}
