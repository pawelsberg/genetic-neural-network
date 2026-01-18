using System;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCasesSequentialContainerQualityMeterTests;

public class ConstructorTests
{
    [Fact]
    public void SetsTestCaseListViaProperty()
    {
        TestCaseList testCaseList = TestHelper.CreateTestCaseList();
        double goodDifference = 0.001;

        TestCasesSequentialContainerQualityMeter container = new TestCasesSequentialContainerQualityMeter(goodDifference, TestHelper.CreateFactory());
        container.TestCaseList = testCaseList;

        Assert.Same(testCaseList, container.TestCaseList);
    }

    [Fact]
    public void SetsPropagationsViaProperty()
    {
        int propagations = 10;
        double goodDifference = 0.001;

        TestCasesSequentialContainerQualityMeter container = new TestCasesSequentialContainerQualityMeter(goodDifference, TestHelper.CreateFactory());
        container.Propagations = propagations;

        Assert.Equal(propagations, container.Propagations);
    }

    [Fact]
    public void TestCaseListReturnsNullWhenNotSet()
    {
        double goodDifference = 0.001;
        TestCasesSequentialContainerQualityMeter container = new TestCasesSequentialContainerQualityMeter(goodDifference, TestHelper.CreateFactory());

        Assert.Null(container.TestCaseList);
    }

    [Fact]
    public void PropagationsReturnsNullWhenNotSet()
    {
        double goodDifference = 0.001;
        TestCasesSequentialContainerQualityMeter container = new TestCasesSequentialContainerQualityMeter(goodDifference, TestHelper.CreateFactory());

        Assert.Null(container.Propagations);
    }
}
