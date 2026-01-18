using System;
using System.Linq;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCasesContainerQualityMeterTests;

public class ChildrenTests
{
    [Fact]
    public void CreatesTestCaseMeters()
    {
        TestCaseList testCaseList = TestHelper.CreateTestCaseList();
        TestCasesContainerQualityMeter container = new TestCasesContainerQualityMeter(TestHelper.CreateFactory());
        container.TestCaseList = testCaseList;
        container.Propagations = 10;

        System.Collections.Generic.List<Pawelsberg.GeneticNeuralNetwork.Model.Genetics.QualityMeter<Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking.Network>> children = container.Children;
        Assert.Equal(2, children.Count);
        Assert.All(children, c => Assert.IsType<TestCaseNetworkQualityMeter>(c));
    }

    [Fact]
    public void WithPropagationRange_CreatesMetersForEachPropagation()
    {
        TestCaseList testCaseList = TestHelper.CreateTestCaseList();
        TestCasesContainerQualityMeter container = new TestCasesContainerQualityMeter(TestHelper.CreateFactory(), 2);
        container.TestCaseList = testCaseList;
        container.Propagations = 5;

        System.Collections.Generic.List<Pawelsberg.GeneticNeuralNetwork.Model.Genetics.QualityMeter<Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking.Network>> children = container.Children;
        Assert.Equal(6, children.Count);
    }

    [Fact]
    public void ReturnsNullWhenNotConfigured()
    {
        TestCasesContainerQualityMeter container = new TestCasesContainerQualityMeter(TestHelper.CreateFactory());

        Assert.Null(container.Children);
    }
}
