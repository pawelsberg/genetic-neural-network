using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.Mutating.DiskStoring;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.Mutating;

public class NodeReordererNetworkMutator : Mutator<Network>, INetworkMutatorTextConvertible
{
    public static string TextName = "NodeReorderer";

    public override MutationDescription Mutate(Network network)
    {
        int node1Index = RandomGenerator.Random.Next(network.Nodes.Count);
        Node node1 = network.Nodes[node1Index];
        int node2Index = RandomGenerator.Random.Next(network.Nodes.Count);
        Node node2 = network.Nodes[node2Index];
        // it could be the same then it will work "safe" - will do nothing
        network.Nodes[node1Index] = node2;
        network.Nodes[node2Index] = node1;
        return new MutationDescription() { Text = "NodeReorderer" };
    }

    public string ToText() => TextName;
}
