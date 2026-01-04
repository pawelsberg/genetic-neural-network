using System.Globalization;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring;

public class TestCasesIfAllGoodNetworkQualityMeterTests
{
    [Fact]
    public void TextName_IsIfAllGood()
    {
        Assert.Equal("IfAllGood", TestCasesIfAllGoodNetworkQualityMeter.TextName);
    }

    [Fact]
    public void Constructor_SetsProperties()
    {
        var parent = new QualityMeter<Network>(null);
        double goodDifference = 0.01;

        var meter = new TestCasesIfAllGoodNetworkQualityMeter(parent, goodDifference);

        Assert.Equal(goodDifference, meter.GoodDifference);
        Assert.Same(parent, meter.Parent);
    }

    [Fact]
    public void ToText_FormatsCorrectly()
    {
        var parent = new QualityMeter<Network>(null);
        var meter = new TestCasesIfAllGoodNetworkQualityMeter(parent, 0.001);

        string text = meter.ToText();

        Assert.Equal("IfAllGood(0.001)", text);
    }

    [Fact]
    public void ToText_UsesInvariantCulture()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            var parent = new QualityMeter<Network>(null);
            var meter = new TestCasesIfAllGoodNetworkQualityMeter(parent, 0.5);

            string text = meter.ToText();

            Assert.Equal("IfAllGood(0.5)", text);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }
}
