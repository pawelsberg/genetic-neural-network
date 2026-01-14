using System.Linq;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCasesContainerQualityMeterTests;

public class AddStaticChildTests
{
    [Fact]
    public void AddsToStaticChildren()
    {
        TestCaseList testCaseList = TestHelper.CreateTestCaseList();
        TestCasesContainerQualityMeter container = new TestCasesContainerQualityMeter(testCaseList, 10, TestHelper.CreateFactory());
        TestCasesIfAllGoodNetworkQualityMeter staticChild = new TestCasesIfAllGoodNetworkQualityMeter(container, 0.001);

        container.AddStaticChild(staticChild);

        System.Collections.Generic.List<Pawelsberg.GeneticNeuralNetwork.Model.Genetics.QualityMeter<Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking.Network>> staticChildren = container.GetStaticChildren().ToList();
        Assert.Single(staticChildren);
        Assert.Same(staticChild, staticChildren[0]);
    }
}
