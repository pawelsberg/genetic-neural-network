using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCasesIfAllGoodNetworkQualityMeterTests;

public class ConstructorTests
{
    [Fact]
    public void SetsGoodDifference()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        double goodDifference = 0.01;

        TestCasesIfAllGoodNetworkQualityMeter meter = new TestCasesIfAllGoodNetworkQualityMeter(parent, goodDifference);

        Assert.Equal(goodDifference, meter.GoodDifference);
    }

    [Fact]
    public void SetsParent()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        double goodDifference = 0.01;

        TestCasesIfAllGoodNetworkQualityMeter meter = new TestCasesIfAllGoodNetworkQualityMeter(parent, goodDifference);

        Assert.Same(parent, meter.Parent);
    }
}
