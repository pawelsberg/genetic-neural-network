using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCaseGoodResultNetworkQualityMeterTests;

public class TextNameTests
{
    [Fact]
    public void IsGoodResult()
    {
        Assert.Equal("GoodResult", TestCaseGoodResultNetworkQualityMeter.TextName);
    }
}
