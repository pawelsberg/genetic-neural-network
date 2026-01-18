using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCasesSequentialContainerQualityMeterTests;

public class ImplementsITestCasesQualityMeterContainerTests
{
    [Fact]
    public void IsAssignable()
    {
        TestCasesSequentialContainerQualityMeter container = new TestCasesSequentialContainerQualityMeter(0.001, TestHelper.CreateFactory());

        Assert.IsAssignableFrom<INetworkQualityMeterWithTestCaseList>(container);
    }
}
