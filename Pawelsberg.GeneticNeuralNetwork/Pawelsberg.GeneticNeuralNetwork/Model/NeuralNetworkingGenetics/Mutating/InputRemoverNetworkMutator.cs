using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.Mutating;

public class InputRemoverNetworkMutator : Mutator<Network>
{
    public override MutationDescription Mutate(Network network)
    {
        if (network.Inputs.Count > 1)
        {
            int inputIndex = RandomGenerator.Random.Next(network.Inputs.Count);
            Synapse inputSynapse = network.Inputs[inputIndex];
            Neuron inputNeuron = network.Nodes.OfType<Neuron>().Single(nr => nr.Inputs.Contains(inputSynapse));
            inputNeuron.Inputs.Remove(inputSynapse);
            network.Inputs.Remove(inputSynapse);
            return new MutationDescription() { Text = "InputRemover: input removed" };
        }
        return new MutationDescription() { Text = "InputRemover: input not removed - only one left" };
    }
}



