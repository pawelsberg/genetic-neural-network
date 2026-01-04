using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.Mutating.DiskStoring;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.Mutating;

public class NeuronModifierNetworkMutator : Mutator<Network>, INetworkMutatorTextConvertible
{
    public static string TextName = "NeuronModifier";

    public override MutationDescription Mutate(Network network)
    {
        if (RandomGenerator.Random.NextDouble() < 0.5d)
            TryModifyRandomNeuron(network); // if randomly selected neuron has no inputs - no change will be made
        else
            TryModifyRandomNeuronInteger(network); // if randomly selected neuron has no inputs - no change will be made
        return new MutationDescription() { Text = "NeuronModifier" };
    }

    private void TryModifyRandomNeuronInteger(Network network)
    {
        int index = RandomGenerator.Random.Next(network.Nodes.Count);
        if (network.Nodes[index] is Neuron)
            TryRandomlyModifyInputMultiplierInteger(network.Nodes[index] as Neuron);
    }

    private void TryRandomlyModifyInputMultiplierInteger(Neuron neuron)
    {
        if (neuron.Inputs.Count > 0)
        {
            int index = RandomGenerator.Random.Next(neuron.Inputs.Count);
            neuron.InputMultiplier[index] = RandomGenerator.RandomizeInteger((int)Math.Min(Math.Max(neuron.InputMultiplier[index], int.MinValue), int.MaxValue));
        }
    }

    private void TryModifyRandomNeuron(Network network)
    {
        int index = RandomGenerator.Random.Next(network.Nodes.Count);
        if (network.Nodes[index] is Neuron)
            TryRandomlyModifyInputMultiplier(network.Nodes[index] as Neuron);
    }

    public void TryRandomlyModifyInputMultiplier(Neuron neuron)
    {
        if (neuron.Inputs.Count > 0)
        {
            int index = RandomGenerator.Random.Next(neuron.Inputs.Count);
            neuron.InputMultiplier[index] = RandomGenerator.Randomize(neuron.InputMultiplier[index]);
        }
    }

    public string ToText() => TextName;
}
