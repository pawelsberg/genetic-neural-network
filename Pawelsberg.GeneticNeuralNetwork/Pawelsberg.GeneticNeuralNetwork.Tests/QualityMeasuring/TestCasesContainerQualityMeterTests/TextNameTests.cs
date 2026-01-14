using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCasesContainerQualityMeterTests;

public class TextNameTests
{
    [Fact]
    public void IsTestCases()
    {
        Assert.Equal("TestCases", TestCasesContainerQualityMeter.TextName);
    }
}
