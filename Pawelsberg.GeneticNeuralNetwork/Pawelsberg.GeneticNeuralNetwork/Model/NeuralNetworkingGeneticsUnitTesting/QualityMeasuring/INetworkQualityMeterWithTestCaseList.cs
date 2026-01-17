using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;

/// <summary>
/// Interface for quality meter containers that depend on propagations and test case list.
/// This allows NetworkSimulation to update parameters without type checking.
/// </summary>
public interface INetworkQualityMeterWithTestCaseList
{
    TestCaseList TestCaseList { get; set; }
}
