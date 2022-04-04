using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting.DiskStoring;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting.Mutating;

public class BackPropagationNetworkMutator : Mutator<Network>
{
    public TestCaseList TestCaseList { get; private set; }
    public int Propagations { get; private set; }
    public double Strength { get; private set; }

    public BackPropagationNetworkMutator(TestCaseList testCaseList, int propagations)
    {
        TestCaseList = testCaseList;
        Propagations = propagations;
        Strength = 0.1d;
    }

    public override MutationDescription Mutate(Network network)
    {
        if (network.Inputs.Count < TestCaseList.TestCases.Max(tc => tc.Inputs.Count)
            || network.Outputs.Count < TestCaseList.TestCases.Max(tc => tc.Outputs.Count))
            return new MutationDescription() { Text = "No Back Propagation" };

        List<Synapse> notOutSynapses = network.GetAllSynapses().Where(s => !network.Outputs.Contains(s)).ToList();
        int notOutSynapseIndex = RandomGenerator.Random.Next(notOutSynapses.Count);

        Synapse notOutSynapse = notOutSynapses[notOutSynapseIndex];

        network.BackPropagation(TestCaseList, Propagations, notOutSynapse);


        return new MutationDescription() { Text = "BackPropagation" };
    }
}
