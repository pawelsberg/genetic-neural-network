using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCasesSequentialContainerQualityMeterTests;

public class ImplementsINetworkQualityMeterContainerTextConvertibleTests
{
    [Fact]
    public void IsAssignable()
    {
        TestCaseList testCaseList = TestHelper.CreateTestCaseList();
        TestCasesSequentialContainerQualityMeter container = new TestCasesSequentialContainerQualityMeter(testCaseList, 10, 0.001, TestHelper.CreateFactory());

        Assert.IsAssignableFrom<INetworkQualityMeterContainerTextConvertible>(container);
    }
}
