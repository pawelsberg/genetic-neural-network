using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCasesContainerQualityMeterTests;

public class ImplementsINetworkQualityMeterContainerTextConvertibleTests
{
    [Fact]
    public void IsAssignable()
    {
        TestCaseList testCaseList = TestHelper.CreateTestCaseList();
        TestCasesContainerQualityMeter container = new TestCasesContainerQualityMeter(testCaseList, 10, TestHelper.CreateFactory());

        Assert.IsAssignableFrom<INetworkQualityMeterContainerTextConvertible>(container);
    }
}
