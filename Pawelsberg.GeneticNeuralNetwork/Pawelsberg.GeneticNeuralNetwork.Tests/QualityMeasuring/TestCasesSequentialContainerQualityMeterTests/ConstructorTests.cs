using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCasesSequentialContainerQualityMeterTests;

public class ConstructorTests
{
    [Fact]
    public void SetsTestCaseList()
    {
        TestCaseList testCaseList = TestHelper.CreateTestCaseList();
        int propagations = 10;
        double goodDifference = 0.001;

        TestCasesSequentialContainerQualityMeter container = new TestCasesSequentialContainerQualityMeter(testCaseList, propagations, goodDifference, TestHelper.CreateFactory());

        Assert.Same(testCaseList, container.TestCaseList);
    }

    [Fact]
    public void SetsPropagations()
    {
        TestCaseList testCaseList = TestHelper.CreateTestCaseList();
        int propagations = 10;
        double goodDifference = 0.001;

        TestCasesSequentialContainerQualityMeter container = new TestCasesSequentialContainerQualityMeter(testCaseList, propagations, goodDifference, TestHelper.CreateFactory());

        Assert.Equal(propagations, container.Propagations);
    }
}
