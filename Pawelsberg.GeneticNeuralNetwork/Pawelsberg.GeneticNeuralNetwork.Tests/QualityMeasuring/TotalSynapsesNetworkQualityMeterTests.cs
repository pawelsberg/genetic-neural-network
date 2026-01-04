using System.Globalization;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.QualityMeasuring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring;

public class TotalSynapsesNetworkQualityMeterTests
{
    [Fact]
    public void TextName_IsTotalSynapses()
    {
        Assert.Equal("TotalSynapses", TotalSynapsesNetworkQualityMeter.TextName);
    }

    [Fact]
    public void Constructor_SetsProperties()
    {
        var parent = new QualityMeter<Network>(null);
        double qualityForOneSynapse = 5.0;

        var meter = new TotalSynapsesNetworkQualityMeter(parent, qualityForOneSynapse);

        Assert.Equal(qualityForOneSynapse, meter.QualityForOneSynapse);
        Assert.Same(parent, meter.Parent);
    }

    [Fact]
    public void ToText_FormatsCorrectly()
    {
        var parent = new QualityMeter<Network>(null);
        var meter = new TotalSynapsesNetworkQualityMeter(parent, 5.0);

        string text = meter.ToText();

        Assert.Equal("TotalSynapses(5)", text);
    }

    [Fact]
    public void ToText_UsesInvariantCulture()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            var parent = new QualityMeter<Network>(null);
            var meter = new TotalSynapsesNetworkQualityMeter(parent, 2.5);

            string text = meter.ToText();

            Assert.Equal("TotalSynapses(2.5)", text);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }
}
