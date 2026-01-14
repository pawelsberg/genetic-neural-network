using System.Globalization;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.QualityMeasuring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.NoLoopsNetworkQualityMeterTests;

public class ToTextTests
{
    [Fact]
    public void FormatsCorrectly()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        NoLoopsNetworkQualityMeter meter = new NoLoopsNetworkQualityMeter(parent, 50.0);

        string text = meter.ToText();

        Assert.Equal("NoLoops(50)", text);
    }

    [Fact]
    public void UsesInvariantCulture()
    {
        CultureInfo originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            QualityMeter<Network> parent = new QualityMeter<Network>(null);
            NoLoopsNetworkQualityMeter meter = new NoLoopsNetworkQualityMeter(parent, 2.5);

            string text = meter.ToText();

            Assert.Equal("NoLoops(2.5)", text);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }
}
