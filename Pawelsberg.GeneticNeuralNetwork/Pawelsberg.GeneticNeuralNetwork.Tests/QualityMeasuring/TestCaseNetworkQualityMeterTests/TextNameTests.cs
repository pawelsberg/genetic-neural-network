using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCaseNetworkQualityMeterTests;

public class TextNameTests
{
    [Fact]
    public void IsTestCase()
    {
        Assert.Equal("TestCase", TestCaseNetworkQualityMeter.TextName);
    }
}
