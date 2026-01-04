using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.Mutating;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.Mutating.DiskStoring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.Mutating;

public class SynapseAdderNetworkMutator : Mutator<Network>, IUpdatableNetworkMutator, INetworkMutatorTextConvertible
{
    public static string TextName = "SynapseAdder";

    public int MaxSynapses { get; set; }
    public SynapseAdderNetworkMutator(int maxSynapses)
    {
        MaxSynapses = maxSynapses;
    }

    public override MutationDescription Mutate(Network network)
    {
        if (RandomGenerator.Random.NextDouble() < 0.5d)
            TryInsertRandomSynapse(network);
        else
            TryInsertRandomSynapseInteger(network);
        return new MutationDescription() { Text = "SynapseAdder" };
    }

    private void TryInsertRandomSynapseInteger(Network network)
    {
        if (network.SynapseCount() < MaxSynapses)
        {
            // picking two random nodes (those could be the same - it is fine - we will have loop)
            int startIndex = RandomGenerator.Random.Next(network.Nodes.Count);
            int endIndex = RandomGenerator.Random.Next(network.Nodes.Count);

            if (network.Nodes[endIndex] is Neuron)
            {
                Synapse synapse = new Synapse();
                network.Nodes[startIndex].AddOutput(synapse);
                (network.Nodes[endIndex] as Neuron).AddInput(synapse, RandomGenerator.RandomInteger());
            }
        }
    }
    private void TryInsertRandomSynapse(Network network)
    {
        if (network.SynapseCount() < MaxSynapses)
        {
            // picking two random nodes (those could be the same - it is fine - we will have loop)
            int startIndex = RandomGenerator.Random.Next(network.Nodes.Count);
            int endIndex = RandomGenerator.Random.Next(network.Nodes.Count);

            if (network.Nodes[endIndex] is Neuron)
            {
                Synapse synapse = new Synapse();
                network.Nodes[startIndex].AddOutput(synapse);
                (network.Nodes[endIndex] as Neuron).AddInput(synapse, RandomGenerator.Randomize(0d));
            }
        }
    }

    public void UpdateParameters(int maxNodes, int maxSynapses, int propagations, TestCaseList testCaseList)
    {
        MaxSynapses = maxSynapses;
    }

    public string ToText() => TextName;
}
