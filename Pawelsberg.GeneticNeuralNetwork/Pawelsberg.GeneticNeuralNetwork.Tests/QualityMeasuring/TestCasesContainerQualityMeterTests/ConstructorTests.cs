using System;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCasesContainerQualityMeterTests;

public class ConstructorTests
{
    [Fact]
    public void SetsTestCaseListViaProperty()
    {
        TestCaseList testCaseList = TestHelper.CreateTestCaseList();

        TestCasesContainerQualityMeter container = new TestCasesContainerQualityMeter(TestHelper.CreateFactory());
        container.TestCaseList = testCaseList;

        Assert.Same(testCaseList, container.TestCaseList);
    }

    [Fact]
    public void SetsPropagationsViaProperty()
    {
        int propagations = 10;

        TestCasesContainerQualityMeter container = new TestCasesContainerQualityMeter(TestHelper.CreateFactory());
        container.Propagations = propagations;

        Assert.Equal(propagations, container.Propagations);
    }

    [Fact]
    public void TestCaseListReturnsNullWhenNotSet()
    {
        TestCasesContainerQualityMeter container = new TestCasesContainerQualityMeter(TestHelper.CreateFactory());

        Assert.Null(container.TestCaseList);
    }

    [Fact]
    public void PropagationsReturnsNullWhenNotSet()
    {
        TestCasesContainerQualityMeter container = new TestCasesContainerQualityMeter(TestHelper.CreateFactory());

        Assert.Null(container.Propagations);
    }
}
