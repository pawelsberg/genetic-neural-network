namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting.QualityMeasuring;

/// <summary>
/// Interface for quality meters that have a Propagations property (e.g., TestCaseNetworkQualityMeter).
/// Used by the text extension for serialization of static quality meter structures.
/// </summary>
public interface INetworkQualityMeterWithPropagations
{
    int Propagations { get; }
}
