using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting.DiskStoring;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting.Mutating
{
    internal interface IUpdatableNetworkMutator
    {
        void UpdateParameters(int maxNodes, int maxSynapses, int propagations, TestCaseList testCaseList);
    }
}
