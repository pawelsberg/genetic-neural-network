using System.Linq;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCasesSequentialContainerQualityMeterTests;

public class AddStaticChildTests
{
    [Fact]
    public void AddsStaticChild()
    {
        TestCasesSequentialContainerQualityMeter container = new TestCasesSequentialContainerQualityMeter(0.001, TestHelper.CreateFactory());
        TestCasesIfAllGoodNetworkQualityMeter staticChild = new TestCasesIfAllGoodNetworkQualityMeter(container, 0.001);

        container.AddStaticChild(staticChild);

        Assert.Single(container.GetStaticChildren());
        Assert.Same(staticChild, container.GetStaticChildren().First());
    }

    [Fact]
    public void AddsMultipleStaticChildren()
    {
        TestCasesSequentialContainerQualityMeter container = new TestCasesSequentialContainerQualityMeter(0.001, TestHelper.CreateFactory());
        TestCasesIfAllGoodNetworkQualityMeter staticChild1 = new TestCasesIfAllGoodNetworkQualityMeter(container, 0.001);
        TestCasesIfAllGoodNetworkQualityMeter staticChild2 = new TestCasesIfAllGoodNetworkQualityMeter(container, 0.002);

        container.AddStaticChild(staticChild1);
        container.AddStaticChild(staticChild2);

        Assert.Equal(2, container.GetStaticChildren().Count());
    }
}
