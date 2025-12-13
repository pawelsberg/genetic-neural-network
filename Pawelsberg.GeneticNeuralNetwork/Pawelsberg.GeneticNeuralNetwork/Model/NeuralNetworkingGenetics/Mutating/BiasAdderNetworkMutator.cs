using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting.Mutating;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting.Mutating.DiskStoring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting.DiskStoring;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.Mutating;

public class BiasAdderNetworkMutator : Mutator<Network>, IUpdatableNetworkMutator, INetworkMutatorTextConvertible
{
    public static string TextName = "BiasAdder";

    public int MaxNodes { get; set; }

    public BiasAdderNetworkMutator(int maxNodes)
    {
        MaxNodes = maxNodes;
    }

    public override MutationDescription Mutate(Network network)
    {
        if (network.Nodes.Count < MaxNodes)
        {
            int insertIndex = RandomGenerator.Random.Next(network.Nodes.Count);
            network.Nodes.Insert(insertIndex, new Bias());
        }
        return new MutationDescription() { Text = "BiasAdder" };
    }

    public void UpdateParameters(int maxNodes, int maxSynapses, int propagations, TestCaseList testCaseList)
    {
        MaxNodes = maxNodes;
    }

    public string ToText() => TextName;
}
