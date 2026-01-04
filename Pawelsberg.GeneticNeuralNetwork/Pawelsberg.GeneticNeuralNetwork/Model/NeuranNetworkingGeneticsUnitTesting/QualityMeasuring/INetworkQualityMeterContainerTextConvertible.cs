using System.Text;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting.QualityMeasuring;

/// <summary>
/// Interface for quality meter containers that need special serialization handling.
/// Containers have a different text format than leaf meters.
/// </summary>
public interface INetworkQualityMeterContainerTextConvertible
{
    /// <summary>
    /// Write the container's serialized representation to a StringBuilder.
    /// </summary>
    void WriteToText(StringBuilder sb);
}
