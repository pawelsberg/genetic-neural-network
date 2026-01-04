using System.Globalization;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting.QualityMeasuring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring;

public class TestCasesTotalTimeNetworkQualityMeterTests
{
    [Fact]
    public void TextName_IsTotalTime()
    {
        Assert.Equal("TotalTime", TestCasesTotalTimeNetworkQualityMeter.TextName);
    }

    [Fact]
    public void Constructor_SetsProperties()
    {
        var parent = new QualityMeter<Network>(null);
        double qualityForOneMs = 100.0;

        var meter = new TestCasesTotalTimeNetworkQualityMeter(parent, qualityForOneMs);

        Assert.Equal(qualityForOneMs, meter.QualityForOneMs);
        Assert.Same(parent, meter.Parent);
    }

    [Fact]
    public void ToText_FormatsCorrectly()
    {
        var parent = new QualityMeter<Network>(null);
        var meter = new TestCasesTotalTimeNetworkQualityMeter(parent, 100.0);

        string text = meter.ToText();

        Assert.Equal("TotalTime(100)", text);
    }

    [Fact]
    public void ToText_UsesInvariantCulture()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            var parent = new QualityMeter<Network>(null);
            var meter = new TestCasesTotalTimeNetworkQualityMeter(parent, 2.5);

            string text = meter.ToText();

            Assert.Equal("TotalTime(2.5)", text);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }
}
