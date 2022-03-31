using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.Mutating
{
    public class NeuronAdderNetworkMutator : Mutator<Network>
    {
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
    }
}