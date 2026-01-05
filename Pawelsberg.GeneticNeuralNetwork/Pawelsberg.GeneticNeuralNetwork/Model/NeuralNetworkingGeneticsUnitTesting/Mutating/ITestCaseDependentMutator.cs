using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.Mutating;

/// <summary>
/// Interface for network mutators that depend on propagations and test case list.
/// This allows NetworkSimulation to update test-related parameters.
/// </summary>
public interface ITestCaseDependentMutator
{
    int Propagations { get; set; }
    TestCaseList TestCaseList { get; set; }
}
