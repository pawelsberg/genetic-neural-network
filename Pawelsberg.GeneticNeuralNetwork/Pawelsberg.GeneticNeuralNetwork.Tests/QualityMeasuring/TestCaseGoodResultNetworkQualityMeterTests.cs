using System.Globalization;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting.QualityMeasuring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring;

public class TestCaseGoodResultNetworkQualityMeterTests
{
    [Fact]
    public void TextName_IsGoodResult()
    {
        Assert.Equal("GoodResult", TestCaseGoodResultNetworkQualityMeter.TextName);
    }

    [Fact]
    public void Constructor_SetsProperties()
    {
        var parent = new QualityMeter<Network>(null);
        double goodDifference = 0.01;
        double qualityForGoodResult = 10.0;

        var meter = new TestCaseGoodResultNetworkQualityMeter(parent, goodDifference, qualityForGoodResult);

        Assert.Equal(goodDifference, meter.GoodDifference);
        Assert.Equal(qualityForGoodResult, meter.QualityForGoodResult);
        Assert.Same(parent, meter.Parent);
    }

    [Fact]
    public void ToText_FormatsCorrectly()
    {
        var parent = new QualityMeter<Network>(null);
        var meter = new TestCaseGoodResultNetworkQualityMeter(parent, 0.001, 10.0);

        string text = meter.ToText();

        Assert.Equal("GoodResult(10,0.001)", text);
    }

    [Fact]
    public void ToText_UsesInvariantCulture()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            var parent = new QualityMeter<Network>(null);
            var meter = new TestCaseGoodResultNetworkQualityMeter(parent, 0.5, 2.5);

            string text = meter.ToText();

            Assert.Equal("GoodResult(2.5,0.5)", text);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }
}
