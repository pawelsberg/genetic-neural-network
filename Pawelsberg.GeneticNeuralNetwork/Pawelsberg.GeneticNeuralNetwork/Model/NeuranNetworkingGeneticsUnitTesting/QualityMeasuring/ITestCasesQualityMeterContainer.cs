using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting.DiskStoring;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting.QualityMeasuring;

/// <summary>
/// Interface for quality meter containers that depend on propagations and test case list.
/// This allows NetworkSimulation to update parameters without type checking.
/// </summary>
public interface ITestCasesQualityMeterContainer
{
    int Propagations { get; set; }
    TestCaseList TestCaseList { get; set; }
}
