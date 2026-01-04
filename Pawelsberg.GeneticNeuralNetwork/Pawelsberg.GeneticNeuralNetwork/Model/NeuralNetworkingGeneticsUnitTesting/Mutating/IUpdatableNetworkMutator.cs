using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.Mutating
{
    internal interface IUpdatableNetworkMutator
    {
        void UpdateParameters(int maxNodes, int maxSynapses, int propagations, TestCaseList testCaseList);
    }
}
