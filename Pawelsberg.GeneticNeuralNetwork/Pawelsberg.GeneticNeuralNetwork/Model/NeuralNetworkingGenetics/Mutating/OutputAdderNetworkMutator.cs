using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting.Mutating;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting.Mutating.DiskStoring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting.DiskStoring;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.Mutating;

public class OutputAdderNetworkMutator : Mutator<Network>, IUpdatableNetworkMutator, INetworkMutatorTextConvertible
{
    public static string TextName = "OutputAdder";

    public int MaxSynapses { get; set; }
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

    public void UpdateParameters(int maxNodes, int maxSynapses, int propagations, TestCaseList testCaseList)
    {
        MaxSynapses = maxSynapses;
    }

    public string ToText() => TextName;
}

