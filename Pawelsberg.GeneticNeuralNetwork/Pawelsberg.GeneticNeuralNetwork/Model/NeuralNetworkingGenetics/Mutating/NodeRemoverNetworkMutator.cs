using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.Mutating.DiskStoring;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.Mutating;

public class NodeRemoverNetworkMutator : Mutator<Network>, INetworkMutatorTextConvertible
{
    public static string TextName = "NodeRemover";

    public override MutationDescription Mutate(Network network)
    {
        int index = RandomGenerator.Random.Next(network.Nodes.Count);
        Node nodeToRemove = network.Nodes[index];
        if (network.Nodes.Count > 1 && nodeToRemove.Outputs.Count == 0 && (nodeToRemove is Bias || nodeToRemove is Neuron && (nodeToRemove as Neuron).Inputs.Count == 0))
            network.Nodes.Remove(nodeToRemove); // remove because disconnected from the network and there are other nodes still left

        return new MutationDescription() { Text = "NodeRemover" };
    }

    public string ToText() => TextName;
}
