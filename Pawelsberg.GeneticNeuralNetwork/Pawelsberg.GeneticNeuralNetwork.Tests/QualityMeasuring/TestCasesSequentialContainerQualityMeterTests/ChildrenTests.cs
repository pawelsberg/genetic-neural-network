using System;
using System.Collections.Generic;
using System.Linq;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCasesSequentialContainerQualityMeterTests;

public class ChildrenTests
{
    [Fact]
    public void CreatesSequentialStructure()
    {
        TestCaseList testCaseList = TestHelper.CreateTestCaseList();
        TestCasesSequentialContainerQualityMeter container = new TestCasesSequentialContainerQualityMeter(0.001, TestHelper.CreateFactory());
        container.TestCaseList = testCaseList;
        container.Propagations = 10;

        List<QualityMeter<Network>> children = container.Children;
        Assert.Equal(2, children.Count);
        Assert.IsType<TestCaseNetworkQualityMeter>(children[0]);
        Assert.IsAssignableFrom<TestCasesIfAllGoodNetworkQualityMeter>(children[1]);
    }

    [Fact]
    public void ReturnsNullWhenNotConfigured()
    {
        TestCasesSequentialContainerQualityMeter container = new TestCasesSequentialContainerQualityMeter(0.001, TestHelper.CreateFactory());

        Assert.Null(container.Children);
    }

    [Fact]
    public void IncludesStaticChildrenAfterTestCases()
    {
        TestCaseList testCaseList = TestHelper.CreateTestCaseList();
        TestCasesSequentialContainerQualityMeter container = new TestCasesSequentialContainerQualityMeter(0.001, TestHelper.CreateFactory());
        container.TestCaseList = testCaseList;
        container.Propagations = 10;
        TestCasesIfAllGoodNetworkQualityMeter staticChild = new TestCasesIfAllGoodNetworkQualityMeter(container, 0.001);
        container.AddStaticChild(staticChild);

        List<QualityMeter<Network>> children = container.Children;
        
        Assert.Equal(3, children.Count);
        Assert.Same(staticChild, children.Last());
    }
}
