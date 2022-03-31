using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.Mutating
{
    public class OutputRemoverNetworkMutator : Mutator<Network>
    {
        public override MutationDescription Mutate(Network network)
        {
            if (network.Outputs.Count > 1)
            {
                int outputIndex = RandomGenerator.Random.Next(network.Outputs.Count);
                Synapse outputSynapse = network.Outputs[outputIndex];
                Node outputNode = network.Nodes.FirstOrDefault(nd => nd.Outputs.Contains(outputSynapse));
                if (outputNode != null)
                    outputNode.Outputs.Remove(outputSynapse);
                network.Outputs.Remove(outputSynapse);
            }
            return new MutationDescription() { Text = "OutputRemover" };
        }
    }
}