using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCaseDifferenceNetworkQualityMeterTests;

public class ConstructorTests
{
    [Fact]
    public void SetsGoodDifference()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        double goodDifference = 0.01;
        double qualityForOneDiff = 1.0;

        TestCaseDifferenceNetworkQualityMeter meter = new TestCaseDifferenceNetworkQualityMeter(parent, goodDifference, qualityForOneDiff);

        Assert.Equal(goodDifference, meter.GoodDifference);
    }

    [Fact]
    public void SetsQualityForOneDiff()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        double goodDifference = 0.01;
        double qualityForOneDiff = 1.0;

        TestCaseDifferenceNetworkQualityMeter meter = new TestCaseDifferenceNetworkQualityMeter(parent, goodDifference, qualityForOneDiff);

        Assert.Equal(qualityForOneDiff, meter.QualityForOneDiff);
    }

    [Fact]
    public void SetsParent()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        double goodDifference = 0.01;
        double qualityForOneDiff = 1.0;

        TestCaseDifferenceNetworkQualityMeter meter = new TestCaseDifferenceNetworkQualityMeter(parent, goodDifference, qualityForOneDiff);

        Assert.Same(parent, meter.Parent);
    }
}
