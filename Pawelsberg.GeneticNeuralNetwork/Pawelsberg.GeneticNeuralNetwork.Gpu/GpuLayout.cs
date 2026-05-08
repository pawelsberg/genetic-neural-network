namespace Pawelsberg.GeneticNeuralNetwork.Gpu;

/// <summary>
/// Per-run buffer dimensions and offsets. Sizes are determined at GpuRunner construction
/// from observed seed networks + simulation MaxNodes/MaxSynapses caps. MaxInputsPerNode,
/// MaxOutputsPerNode, MaxNetworkInputs, MaxNetworkOutputs are sized to fit observed
/// networks with no separate cap (the only physical bound on per-node fan-in/out is the
/// total synapse count). MutatorMaxNodes / MutatorMaxSynapses are the soft caps mutators
/// respect (matching CPU's Network.MaxNodes / MaxSynapses), independent of buffer sizes.
/// </summary>
public sealed class GpuLayout
{
    public int PopulationSize { get; }
    public int MaxNodes { get; }
    public int MaxSynapses { get; }
    public int MaxInputsPerNode { get; }
    public int MaxOutputsPerNode { get; }
    public int MaxNetworkInputs { get; }
    public int MaxNetworkOutputs { get; }
    public int MaxTestCaseInputs { get; }
    public int MaxTestCaseOutputs { get; }
    public int MutatorMaxNodes { get; }
    public int MutatorMaxSynapses { get; }
    public int Propagations { get; }
    public int TestCaseCount { get; }

    public int OffActiveNodes => 0;
    public int OffNetworkInputCount => 1;
    public int OffNetworkOutputCount => 2;
    public int OffReserved => 3;
    public int HeaderInts => 4;

    public int OffNodeType => HeaderInts;
    public int OffNodeActivation => OffNodeType + MaxNodes;
    public int OffNodeInputCount => OffNodeActivation + MaxNodes;
    public int OffNodeInputSynapse => OffNodeInputCount + MaxNodes;
    public int OffNodeOutputCount => OffNodeInputSynapse + MaxNodes * MaxInputsPerNode;
    public int OffNodeOutputSynapse => OffNodeOutputCount + MaxNodes;
    public int OffNetworkInputSynapse => OffNodeOutputSynapse + MaxNodes * MaxOutputsPerNode;
    public int OffNetworkOutputSynapse => OffNetworkInputSynapse + MaxNetworkInputs;
    public int OffSynapseActive => OffNetworkOutputSynapse + MaxNetworkOutputs;
    public int IntStridePerSpecimen => OffSynapseActive + MaxSynapses;
    public int FloatStridePerSpecimen => MaxNodes * MaxInputsPerNode;

    public GpuLayout(
        int populationSize,
        int maxNodes,
        int maxSynapses,
        int maxInputsPerNode,
        int maxOutputsPerNode,
        int maxNetworkInputs,
        int maxNetworkOutputs,
        int maxTestCaseInputs,
        int maxTestCaseOutputs,
        int mutatorMaxNodes,
        int mutatorMaxSynapses,
        int propagations,
        int testCaseCount)
    {
        PopulationSize = populationSize;
        MaxNodes = maxNodes;
        MaxSynapses = maxSynapses;
        MaxInputsPerNode = maxInputsPerNode;
        MaxOutputsPerNode = maxOutputsPerNode;
        MaxNetworkInputs = maxNetworkInputs;
        MaxNetworkOutputs = maxNetworkOutputs;
        MaxTestCaseInputs = maxTestCaseInputs;
        MaxTestCaseOutputs = maxTestCaseOutputs;
        MutatorMaxNodes = mutatorMaxNodes;
        MutatorMaxSynapses = mutatorMaxSynapses;
        Propagations = propagations;
        TestCaseCount = testCaseCount;
    }
}
