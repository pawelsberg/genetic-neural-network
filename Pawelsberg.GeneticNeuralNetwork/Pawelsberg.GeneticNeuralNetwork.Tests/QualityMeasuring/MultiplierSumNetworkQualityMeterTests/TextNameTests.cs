using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.QualityMeasuring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.MultiplierSumNetworkQualityMeterTests;

public class TextNameTests
{
    [Fact]
    public void IsMultiplierSum()
    {
        Assert.Equal("MultiplierSum", MultiplierSumNetworkQualityMeter.TextName);
    }
}
