using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCaseListNetworkQualityMeterTests;

public class TextNameTests
{
    [Fact]
    public void IsTestCaseList()
    {
        Assert.Equal("TestCaseList", TestCaseListNetworkQualityMeter.TextName);
    }
}
