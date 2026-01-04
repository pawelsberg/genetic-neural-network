using System.Globalization;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting.QualityMeasuring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring;

public class TestCaseSubstractErrorIfGoodNetworkQualityMeterTests
{
    [Fact]
    public void TextName_IsSubstractErrorIfGood()
    {
        Assert.Equal("SubstractErrorIfGood", TestCaseSubstractErrorIfGoodNetworkQualityMeter.TextName);
    }

    [Fact]
    public void Constructor_SetsProperties()
    {
        var parent = new QualityMeter<Network>(null);
        double goodDifference = 0.01;
        double substractedQuality = 0.5;

        var meter = new TestCaseSubstractErrorIfGoodNetworkQualityMeter(parent, goodDifference, substractedQuality);

        Assert.Equal(goodDifference, meter.GoodDifference);
        Assert.Equal(substractedQuality, meter.SubstractedQualityForOneDiff);
        Assert.Same(parent, meter.Parent);
    }

    [Fact]
    public void ToText_FormatsCorrectly()
    {
        var parent = new QualityMeter<Network>(null);
        var meter = new TestCaseSubstractErrorIfGoodNetworkQualityMeter(parent, 0.001, 0.5);

        string text = meter.ToText();

        Assert.Equal("SubstractErrorIfGood(0.5,0.001)", text);
    }

    [Fact]
    public void ToText_UsesInvariantCulture()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            var parent = new QualityMeter<Network>(null);
            var meter = new TestCaseSubstractErrorIfGoodNetworkQualityMeter(parent, 0.5, 2.5);

            string text = meter.ToText();

            Assert.Equal("SubstractErrorIfGood(2.5,0.5)", text);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }
}
