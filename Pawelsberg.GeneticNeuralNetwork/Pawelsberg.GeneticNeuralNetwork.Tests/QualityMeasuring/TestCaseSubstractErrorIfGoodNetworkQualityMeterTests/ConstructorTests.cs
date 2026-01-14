using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCaseSubstractErrorIfGoodNetworkQualityMeterTests;

public class ConstructorTests
{
    [Fact]
    public void SetsGoodDifference()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        double goodDifference = 0.01;
        double substractedQuality = 0.5;

        TestCaseSubstractErrorIfGoodNetworkQualityMeter meter = new TestCaseSubstractErrorIfGoodNetworkQualityMeter(parent, goodDifference, substractedQuality);

        Assert.Equal(goodDifference, meter.GoodDifference);
    }

    [Fact]
    public void SetsSubstractedQualityForOneDiff()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        double goodDifference = 0.01;
        double substractedQuality = 0.5;

        TestCaseSubstractErrorIfGoodNetworkQualityMeter meter = new TestCaseSubstractErrorIfGoodNetworkQualityMeter(parent, goodDifference, substractedQuality);

        Assert.Equal(substractedQuality, meter.SubstractedQualityForOneDiff);
    }

    [Fact]
    public void SetsParent()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        double goodDifference = 0.01;
        double substractedQuality = 0.5;

        TestCaseSubstractErrorIfGoodNetworkQualityMeter meter = new TestCaseSubstractErrorIfGoodNetworkQualityMeter(parent, goodDifference, substractedQuality);

        Assert.Same(parent, meter.Parent);
    }
}
