using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.Mutating;

public class OutputAdderNetworkMutator : Mutator<Network>
{
    private int MaxSynapses { get; set; }
    public OutputAdderNetworkMutator(int maxSynapses)
    {
        MaxSynapses = maxSynapses;
    }

    public override MutationDescription Mutate(Network network)
    {
        if (network.SynapseCount() < MaxSynapses)
        {
            // picking a random node
            int nodeIndex = RandomGenerator.Random.Next(network.Nodes.Count);

            Synapse synapse = new Synapse();
            network.Nodes[nodeIndex].AddOutput(synapse);
            network.Outputs.Add(synapse);
        }
        return new MutationDescription() { Text = "OutputAdder" };
    }
}

