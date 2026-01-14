using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCaseSubstractErrorIfGoodNetworkQualityMeterTests;

public class TextNameTests
{
    [Fact]
    public void IsSubstractErrorIfGood()
    {
        Assert.Equal("SubstractErrorIfGood", TestCaseSubstractErrorIfGoodNetworkQualityMeter.TextName);
    }
}
