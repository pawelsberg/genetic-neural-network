using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCasesContainerQualityMeterTests;

public class ImplementsINetworkQualityMeterContainerTextConvertibleTests
{
    [Fact]
    public void IsAssignable()
    {
        TestCasesContainerQualityMeter container = new TestCasesContainerQualityMeter(TestHelper.CreateFactory());

        Assert.IsAssignableFrom<INetworkQualityMeterContainerTextConvertible>(container);
    }
}
