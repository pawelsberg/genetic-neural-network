using System.Globalization;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.QualityMeasuring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring;

public class TotalNodesNetworkQualityMeterTests
{
    [Fact]
    public void TextName_IsTotalNodes()
    {
        Assert.Equal("TotalNodes", TotalNodesNetworkQualityMeter.TextName);
    }

    [Fact]
    public void Constructor_SetsProperties()
    {
        var parent = new QualityMeter<Network>(null);
        double qualityForOneNode = 10.0;

        var meter = new TotalNodesNetworkQualityMeter(parent, qualityForOneNode);

        Assert.Equal(qualityForOneNode, meter.QualityForOneNode);
        Assert.Same(parent, meter.Parent);
    }

    [Fact]
    public void ToText_FormatsCorrectly()
    {
        var parent = new QualityMeter<Network>(null);
        var meter = new TotalNodesNetworkQualityMeter(parent, 10.0);

        string text = meter.ToText();

        Assert.Equal("TotalNodes(10)", text);
    }

    [Fact]
    public void ToText_UsesInvariantCulture()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            var parent = new QualityMeter<Network>(null);
            var meter = new TotalNodesNetworkQualityMeter(parent, 2.5);

            string text = meter.ToText();

            Assert.Equal("TotalNodes(2.5)", text);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }
}
