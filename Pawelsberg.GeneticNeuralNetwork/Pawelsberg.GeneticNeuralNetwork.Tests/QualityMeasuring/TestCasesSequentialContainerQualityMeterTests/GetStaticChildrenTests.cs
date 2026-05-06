using System.Linq;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCasesSequentialContainerQualityMeterTests;

public class GetStaticChildrenTests
{
    [Fact]
    public void ReturnsEmptyWhenNoStaticChildren()
    {
        TestCasesSequentialContainerQualityMeter container = new TestCasesSequentialContainerQualityMeter(0.001, TestHelper.CreateFactory());

        Assert.Empty(container.GetStaticChildren());
    }

    [Fact]
    public void ReturnsAddedStaticChildren()
    {
        TestCasesSequentialContainerQualityMeter container = new TestCasesSequentialContainerQualityMeter(0.001, TestHelper.CreateFactory());
        TestCasesIfAllGoodNetworkQualityMeter staticChild = new TestCasesIfAllGoodNetworkQualityMeter(container, 0.001);
        container.AddStaticChild(staticChild);

        Assert.Single(container.GetStaticChildren());
        Assert.Same(staticChild, container.GetStaticChildren().First());
    }
}
