using System.Globalization;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.QualityMeasuring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring;

public class NoLoopsNetworkQualityMeterTests
{
    [Fact]
    public void TextName_IsNoLoops()
    {
        Assert.Equal("NoLoops", NoLoopsNetworkQualityMeter.TextName);
    }

    [Fact]
    public void Constructor_SetsProperties()
    {
        var parent = new QualityMeter<Network>(null);
        double qualityForZeroLoops = 50.0;

        var meter = new NoLoopsNetworkQualityMeter(parent, qualityForZeroLoops);

        Assert.Equal(qualityForZeroLoops, meter.QualityForZeroLoops);
        Assert.Same(parent, meter.Parent);
    }

    [Fact]
    public void ToText_FormatsCorrectly()
    {
        var parent = new QualityMeter<Network>(null);
        var meter = new NoLoopsNetworkQualityMeter(parent, 50.0);

        string text = meter.ToText();

        Assert.Equal("NoLoops(50)", text);
    }

    [Fact]
    public void ToText_UsesInvariantCulture()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            var parent = new QualityMeter<Network>(null);
            var meter = new NoLoopsNetworkQualityMeter(parent, 2.5);

            string text = meter.ToText();

            Assert.Equal("NoLoops(2.5)", text);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }
}
