using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting.Mutating;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting.Mutating.DiskStoring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting.DiskStoring;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.Mutating;

public class InputAdderNetworkMutator : Mutator<Network>, IUpdatableNetworkMutator, INetworkMutatorTextConvertible
{
    public static string TextName = "InputAdder";

    public int MaxSynapses { get; set; }
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

    public void UpdateParameters(int maxNodes, int maxSynapses, int propagations, TestCaseList testCaseList)
    {
        MaxSynapses = maxSynapses;
    }

    public string ToText() => TextName;
}

