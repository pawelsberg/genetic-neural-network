using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;

namespace Pawelsberg.GeneticNeuralNetwork.Gpu;

public sealed class FlatGenome
{
    public int[] Ints { get; }
    public float[] Floats { get; }

    private FlatGenome(int intStride, int floatStride, int maxNodes, int offNodeType)
    {
        Ints = new int[intStride];
        Floats = new float[floatStride];
        for (int n = 0; n < maxNodes; n++)
            Ints[offNodeType + n] = GpuConstants.NodeTypeInactive;
    }

    public static FlatGenome Encode(Network network, GpuLayout layout)
    {
        FlatGenome g = new FlatGenome(layout.IntStridePerSpecimen, layout.FloatStridePerSpecimen, layout.MaxNodes, layout.OffNodeType);

        List<Synapse> synapses = network.GetAllSynapses().ToList();
        if (synapses.Count > layout.MaxSynapses)
            throw new Exception($"Network has {synapses.Count} synapses, layout MaxSynapses is {layout.MaxSynapses}");
        if (network.Nodes.Count > layout.MaxNodes)
            throw new Exception($"Network has {network.Nodes.Count} nodes, layout MaxNodes is {layout.MaxNodes}");
        if (network.Inputs.Count > layout.MaxNetworkInputs)
            throw new Exception($"Network has {network.Inputs.Count} inputs, layout MaxNetworkInputs is {layout.MaxNetworkInputs}");
        if (network.Outputs.Count > layout.MaxNetworkOutputs)
            throw new Exception($"Network has {network.Outputs.Count} outputs, layout MaxNetworkOutputs is {layout.MaxNetworkOutputs}");

        for (int i = 0; i < synapses.Count; i++)
            g.Ints[layout.OffSynapseActive + i] = 1;

        g.Ints[layout.OffActiveNodes] = network.Nodes.Count;
        g.Ints[layout.OffNetworkInputCount] = network.Inputs.Count;
        g.Ints[layout.OffNetworkOutputCount] = network.Outputs.Count;

        for (int n = 0; n < network.Nodes.Count; n++)
        {
            Node node = network.Nodes[n];
            int nodeOutputs = node.Outputs.Count;
            if (nodeOutputs > layout.MaxOutputsPerNode)
                throw new Exception($"Node {n} has {nodeOutputs} outputs, layout MaxOutputsPerNode is {layout.MaxOutputsPerNode}");

            if (node is Neuron neuron)
            {
                if (neuron.Inputs.Count > layout.MaxInputsPerNode)
                    throw new Exception($"Neuron {n} has {neuron.Inputs.Count} inputs, layout MaxInputsPerNode is {layout.MaxInputsPerNode}");
                g.Ints[layout.OffNodeType + n] = GpuConstants.NodeTypeNeuron;
                g.Ints[layout.OffNodeActivation + n] = (int)neuron.ActivationFunction;
                g.Ints[layout.OffNodeInputCount + n] = neuron.Inputs.Count;
                for (int i = 0; i < neuron.Inputs.Count; i++)
                {
                    g.Ints[layout.OffNodeInputSynapse + n * layout.MaxInputsPerNode + i] = synapses.IndexOf(neuron.Inputs[i]);
                    g.Floats[n * layout.MaxInputsPerNode + i] = (float)neuron.InputMultiplier[i];
                }
            }
            else if (node is Bias)
            {
                g.Ints[layout.OffNodeType + n] = GpuConstants.NodeTypeBias;
                g.Ints[layout.OffNodeActivation + n] = 0;
                g.Ints[layout.OffNodeInputCount + n] = 0;
            }
            else
                throw new Exception($"Unknown node type at index {n}");

            g.Ints[layout.OffNodeOutputCount + n] = nodeOutputs;
            for (int o = 0; o < nodeOutputs; o++)
                g.Ints[layout.OffNodeOutputSynapse + n * layout.MaxOutputsPerNode + o] = synapses.IndexOf(node.Outputs[o]);
        }

        for (int i = 0; i < network.Inputs.Count; i++)
            g.Ints[layout.OffNetworkInputSynapse + i] = synapses.IndexOf(network.Inputs[i]);
        for (int i = 0; i < network.Outputs.Count; i++)
            g.Ints[layout.OffNetworkOutputSynapse + i] = synapses.IndexOf(network.Outputs[i]);

        return g;
    }

    public static Network Decode(int[] ints, float[] floats, GpuLayout layout)
    {
        Network network = new Network();

        Synapse[] synapseTable = new Synapse[layout.MaxSynapses];
        for (int s = 0; s < layout.MaxSynapses; s++)
            if (ints[layout.OffSynapseActive + s] != 0)
                synapseTable[s] = new Synapse();

        int activeNodes = ints[layout.OffActiveNodes];
        int networkInputCount = ints[layout.OffNetworkInputCount];
        int networkOutputCount = ints[layout.OffNetworkOutputCount];

        for (int n = 0; n < activeNodes; n++)
        {
            int nodeType = ints[layout.OffNodeType + n];
            int outCount = ints[layout.OffNodeOutputCount + n];

            Node resultNode;
            if (nodeType == GpuConstants.NodeTypeNeuron)
            {
                Neuron neuron = new Neuron();
                neuron.ActivationFunction = (ActivationFunction)ints[layout.OffNodeActivation + n];
                int inCount = ints[layout.OffNodeInputCount + n];
                for (int i = 0; i < inCount; i++)
                {
                    int synIdx = ints[layout.OffNodeInputSynapse + n * layout.MaxInputsPerNode + i];
                    Synapse? syn = synapseTable[synIdx];
                    if (syn == null)
                        throw new Exception($"Decode: neuron {n} references inactive synapse {synIdx}");
                    double multiplier = floats[n * layout.MaxInputsPerNode + i];
                    neuron.AddInput(syn, multiplier);
                }
                resultNode = neuron;
            }
            else if (nodeType == GpuConstants.NodeTypeBias)
                resultNode = new Bias();
            else if (nodeType == GpuConstants.NodeTypeInactive)
                continue;
            else
                throw new Exception($"Decode: unknown node type {nodeType} at slot {n}");

            for (int o = 0; o < outCount; o++)
            {
                int synIdx = ints[layout.OffNodeOutputSynapse + n * layout.MaxOutputsPerNode + o];
                Synapse? syn = synapseTable[synIdx];
                if (syn == null)
                    throw new Exception($"Decode: node {n} references inactive output synapse {synIdx}");
                resultNode.AddOutput(syn);
            }
            network.Nodes.Add(resultNode);
        }

        for (int i = 0; i < networkInputCount; i++)
        {
            int synIdx = ints[layout.OffNetworkInputSynapse + i];
            Synapse? syn = synapseTable[synIdx];
            if (syn == null)
                throw new Exception($"Decode: network input {i} references inactive synapse {synIdx}");
            network.Inputs.Add(syn);
        }
        for (int i = 0; i < networkOutputCount; i++)
        {
            int synIdx = ints[layout.OffNetworkOutputSynapse + i];
            Synapse? syn = synapseTable[synIdx];
            if (syn == null)
                throw new Exception($"Decode: network output {i} references inactive synapse {synIdx}");
            network.Outputs.Add(syn);
        }

        return network;
    }
}
