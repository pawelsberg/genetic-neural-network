using System.Globalization;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring;

public class TestCaseDifferenceNetworkQualityMeterTests
{
    [Fact]
    public void TextName_IsDifference()
    {
        Assert.Equal("Difference", TestCaseDifferenceNetworkQualityMeter.TextName);
    }

    [Fact]
    public void Constructor_SetsProperties()
    {
        var parent = new QualityMeter<Network>(null);
        double goodDifference = 0.01;
        double qualityForOneDiff = 1.0;

        var meter = new TestCaseDifferenceNetworkQualityMeter(parent, goodDifference, qualityForOneDiff);

        Assert.Equal(goodDifference, meter.GoodDifference);
        Assert.Equal(qualityForOneDiff, meter.QualityForOneDiff);
        Assert.Same(parent, meter.Parent);
    }

    [Fact]
    public void ToText_FormatsCorrectly()
    {
        var parent = new QualityMeter<Network>(null);
        var meter = new TestCaseDifferenceNetworkQualityMeter(parent, 0.001, 1.0);

        string text = meter.ToText();

        Assert.Equal("Difference(1,0.001)", text);
    }

    [Fact]
    public void ToText_UsesInvariantCulture()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE"); // Uses comma as decimal separator
            var parent = new QualityMeter<Network>(null);
            var meter = new TestCaseDifferenceNetworkQualityMeter(parent, 0.5, 2.5);

            string text = meter.ToText();

            Assert.Equal("Difference(2.5,0.5)", text);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }
}
