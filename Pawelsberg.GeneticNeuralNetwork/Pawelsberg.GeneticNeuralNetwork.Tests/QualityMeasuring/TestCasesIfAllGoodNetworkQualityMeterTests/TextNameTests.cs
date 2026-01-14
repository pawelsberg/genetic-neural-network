using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCasesIfAllGoodNetworkQualityMeterTests;

public class TextNameTests
{
    [Fact]
    public void IsIfAllGood()
    {
        Assert.Equal("IfAllGood", TestCasesIfAllGoodNetworkQualityMeter.TextName);
    }
}
