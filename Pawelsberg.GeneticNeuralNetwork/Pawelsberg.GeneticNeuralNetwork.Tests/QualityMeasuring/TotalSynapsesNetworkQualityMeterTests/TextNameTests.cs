using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.QualityMeasuring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TotalSynapsesNetworkQualityMeterTests;

public class TextNameTests
{
    [Fact]
    public void IsTotalSynapses()
    {
        Assert.Equal("TotalSynapses", TotalSynapsesNetworkQualityMeter.TextName);
    }
}
