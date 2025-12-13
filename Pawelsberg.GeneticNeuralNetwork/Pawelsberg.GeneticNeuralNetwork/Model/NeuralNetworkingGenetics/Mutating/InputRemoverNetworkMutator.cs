using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting.Mutating.DiskStoring;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.Mutating;

public class InputRemoverNetworkMutator : Mutator<Network>, INetworkMutatorTextConvertible
{
    public static string TextName = "InputRemover";

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

    public string ToText() => TextName;
}



