using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.Mutating.DiskStoring;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.Mutating;

public class ActivationFunctionNetworkMutator : Mutator<Network>, INetworkMutatorTextConvertible
{
    public static string TextName = "ActivationFunctionFlipper";

    public override MutationDescription Mutate(Network network)
    {
        int neuronIndex = RandomGenerator.Random.Next(network.Nodes.Count);
        if (network.Nodes[neuronIndex] is Neuron)
        {
            Array values = Enum.GetValues(typeof(ActivationFunction));
            (network.Nodes[neuronIndex] as Neuron).ActivationFunction = (ActivationFunction)values.GetValue(RandomGenerator.Random.Next(values.Length));
        }
        return new MutationDescription() { Text = "ActivationFunctionFlipper" };
    }

    public string ToText() => TextName;
}
