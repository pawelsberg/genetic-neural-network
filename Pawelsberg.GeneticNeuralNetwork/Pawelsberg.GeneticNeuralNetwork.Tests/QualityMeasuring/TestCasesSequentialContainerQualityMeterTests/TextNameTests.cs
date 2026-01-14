using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCasesSequentialContainerQualityMeterTests;

public class TextNameTests
{
    [Fact]
    public void IsTestCasesSequential()
    {
        Assert.Equal("TestCasesSequential", TestCasesSequentialContainerQualityMeter.TextName);
    }
}
