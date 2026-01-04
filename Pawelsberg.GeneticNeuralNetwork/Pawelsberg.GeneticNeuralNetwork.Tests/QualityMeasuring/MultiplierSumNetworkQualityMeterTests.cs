using System.Globalization;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.QualityMeasuring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring;

public class MultiplierSumNetworkQualityMeterTests
{
    [Fact]
    public void TextName_IsMultiplierSum()
    {
        Assert.Equal("MultiplierSum", MultiplierSumNetworkQualityMeter.TextName);
    }

    [Fact]
    public void Constructor_SetsProperties()
    {
        var parent = new QualityMeter<Network>(null);
        double qualityForSumEqOne = 20.0;

        var meter = new MultiplierSumNetworkQualityMeter(parent, qualityForSumEqOne);

        Assert.Equal(qualityForSumEqOne, meter.QualityForSumEqOne);
        Assert.Same(parent, meter.Parent);
    }

    [Fact]
    public void ToText_FormatsCorrectly()
    {
        var parent = new QualityMeter<Network>(null);
        var meter = new MultiplierSumNetworkQualityMeter(parent, 20.0);

        string text = meter.ToText();

        Assert.Equal("MultiplierSum(20)", text);
    }

    [Fact]
    public void ToText_UsesInvariantCulture()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            var parent = new QualityMeter<Network>(null);
            var meter = new MultiplierSumNetworkQualityMeter(parent, 2.5);

            string text = meter.ToText();

            Assert.Equal("MultiplierSum(2.5)", text);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }
}
