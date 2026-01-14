using System.Collections.Generic;
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

        TestCasesSequentialContainerQualityMeter container = new TestCasesSequentialContainerQualityMeter(testCaseList, 10, 0.001, TestHelper.CreateFactory());

        List<QualityMeter<Network>> children = container.Children;
        Assert.Equal(2, children.Count);
        Assert.IsType<TestCaseNetworkQualityMeter>(children[0]);
        Assert.IsAssignableFrom<TestCasesIfAllGoodNetworkQualityMeter>(children[1]);
    }
}
