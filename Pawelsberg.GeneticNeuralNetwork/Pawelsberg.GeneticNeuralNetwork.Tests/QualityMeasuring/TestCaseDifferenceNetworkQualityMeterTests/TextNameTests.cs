using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCaseDifferenceNetworkQualityMeterTests;

public class TextNameTests
{
    [Fact]
    public void IsDifference()
    {
        Assert.Equal("Difference", TestCaseDifferenceNetworkQualityMeter.TextName);
    }
}
