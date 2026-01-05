using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.Mutating.DiskStoring;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.Mutating;

public class NeuronAdderNetworkMutator : Mutator<Network>, IMaxNodesLimitedMutator, INetworkMutatorTextConvertible
{
    public static string TextName = "NeuronAdder";

    public int MaxNodes { get; set; }

    public NeuronAdderNetworkMutator(int maxNodes)
    {
        MaxNodes = maxNodes;
    }

    public override MutationDescription Mutate(Network network)
    {
        if (network.Nodes.Count < MaxNodes)
        {
            int insertIndex = RandomGenerator.Random.Next(network.Nodes.Count);
            network.Nodes.Insert(insertIndex, new Neuron());
        }
        return new MutationDescription() { Text = "NeuronAdder" };
    }

    public string ToText() => TextName;
}
