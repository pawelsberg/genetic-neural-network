using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCasesTotalTimeNetworkQualityMeterTests;

public class TextNameTests
{
    [Fact]
    public void IsTotalTime()
    {
        Assert.Equal("TotalTime", TestCasesTotalTimeNetworkQualityMeter.TextName);
    }
}
