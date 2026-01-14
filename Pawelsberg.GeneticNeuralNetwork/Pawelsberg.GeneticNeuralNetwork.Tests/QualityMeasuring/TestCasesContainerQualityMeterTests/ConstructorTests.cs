using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCasesContainerQualityMeterTests;

public class ConstructorTests
{
    [Fact]
    public void SinglePropagation_SetsTestCaseList()
    {
        TestCaseList testCaseList = TestHelper.CreateTestCaseList();
        int propagations = 10;

        TestCasesContainerQualityMeter container = new TestCasesContainerQualityMeter(testCaseList, propagations, TestHelper.CreateFactory());

        Assert.Same(testCaseList, container.TestCaseList);
    }

    [Fact]
    public void SinglePropagation_SetsPropagations()
    {
        TestCaseList testCaseList = TestHelper.CreateTestCaseList();
        int propagations = 10;

        TestCasesContainerQualityMeter container = new TestCasesContainerQualityMeter(testCaseList, propagations, TestHelper.CreateFactory());

        Assert.Equal(propagations, container.Propagations);
    }

    [Fact]
    public void PropagationRange_SetsTestCaseList()
    {
        TestCaseList testCaseList = TestHelper.CreateTestCaseList();

        TestCasesContainerQualityMeter container = new TestCasesContainerQualityMeter(testCaseList, 5, 15, TestHelper.CreateFactory());

        Assert.Same(testCaseList, container.TestCaseList);
    }

    [Fact]
    public void PropagationRange_SetsPropagations()
    {
        TestCaseList testCaseList = TestHelper.CreateTestCaseList();

        TestCasesContainerQualityMeter container = new TestCasesContainerQualityMeter(testCaseList, 5, 15, TestHelper.CreateFactory());

        Assert.Equal(5, container.Propagations);
    }

    [Fact]
    public void PropagationRange_SetsPropagationRange()
    {
        TestCaseList testCaseList = TestHelper.CreateTestCaseList();

        TestCasesContainerQualityMeter container = new TestCasesContainerQualityMeter(testCaseList, 5, 15, TestHelper.CreateFactory());

        (int from, int to) = container.GetPropagationRange();
        Assert.Equal(5, from);
        Assert.Equal(15, to);
    }
}
