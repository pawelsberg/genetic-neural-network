using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCaseGoodResultNetworkQualityMeterTests;

public class ConstructorTests
{
    [Fact]
    public void SetsGoodDifference()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        double goodDifference = 0.01;
        double qualityForGoodResult = 10.0;

        TestCaseGoodResultNetworkQualityMeter meter = new TestCaseGoodResultNetworkQualityMeter(parent, goodDifference, qualityForGoodResult);

        Assert.Equal(goodDifference, meter.GoodDifference);
    }

    [Fact]
    public void SetsQualityForGoodResult()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        double goodDifference = 0.01;
        double qualityForGoodResult = 10.0;

        TestCaseGoodResultNetworkQualityMeter meter = new TestCaseGoodResultNetworkQualityMeter(parent, goodDifference, qualityForGoodResult);

        Assert.Equal(qualityForGoodResult, meter.QualityForGoodResult);
    }

    [Fact]
    public void SetsParent()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        double goodDifference = 0.01;
        double qualityForGoodResult = 10.0;

        TestCaseGoodResultNetworkQualityMeter meter = new TestCaseGoodResultNetworkQualityMeter(parent, goodDifference, qualityForGoodResult);

        Assert.Same(parent, meter.Parent);
    }
}
