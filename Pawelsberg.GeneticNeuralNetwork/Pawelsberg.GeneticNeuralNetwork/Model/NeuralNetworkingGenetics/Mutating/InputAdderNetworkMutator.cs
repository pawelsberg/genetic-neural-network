using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.Mutating;

public class InputAdderNetworkMutator : Mutator<Network>
{
    private int MaxSynapses { get; set; }
    public InputAdderNetworkMutator(int maxSynapses)
    {
        MaxSynapses = maxSynapses;
    }

    public override MutationDescription Mutate(Network network)
    {
        if (network.SynapseCount() < MaxSynapses)
        {
            // picking two random neurons (those could be the same - it is fine - we will have loop)
            int neuronIndex = RandomGenerator.Random.Next(network.Nodes.Count);

            if (network.Nodes[neuronIndex] is Neuron)
            {
                Synapse synapse = new Synapse();
                (network.Nodes[neuronIndex] as Neuron).AddInput(synapse, RandomGenerator.Randomize(0d));
                network.Inputs.Add(synapse);
            }
            return new MutationDescription() { Text = "InputAdder: input added" };
        }
        return new MutationDescription() { Text = "InputAdder: input not added - maximum number of synapses exist" };
    }
}

