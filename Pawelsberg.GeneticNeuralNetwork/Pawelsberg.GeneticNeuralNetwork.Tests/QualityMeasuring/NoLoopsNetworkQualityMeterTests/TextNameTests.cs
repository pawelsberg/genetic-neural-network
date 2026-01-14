using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.QualityMeasuring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.NoLoopsNetworkQualityMeterTests;

public class TextNameTests
{
    [Fact]
    public void IsNoLoops()
    {
        Assert.Equal("NoLoops", NoLoopsNetworkQualityMeter.TextName);
    }
}
