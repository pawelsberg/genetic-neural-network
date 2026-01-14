using System.Globalization;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCaseSubstractErrorIfGoodNetworkQualityMeterTests;

public class ToTextTests
{
    [Fact]
    public void FormatsCorrectly()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        TestCaseSubstractErrorIfGoodNetworkQualityMeter meter = new TestCaseSubstractErrorIfGoodNetworkQualityMeter(parent, 0.001, 0.5);

        string text = meter.ToText();

        Assert.Equal("SubstractErrorIfGood(0.5,0.001)", text);
    }

    [Fact]
    public void UsesInvariantCulture()
    {
        CultureInfo originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            QualityMeter<Network> parent = new QualityMeter<Network>(null);
            TestCaseSubstractErrorIfGoodNetworkQualityMeter meter = new TestCaseSubstractErrorIfGoodNetworkQualityMeter(parent, 0.5, 2.5);

            string text = meter.ToText();

            Assert.Equal("SubstractErrorIfGood(2.5,0.5)", text);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }
}
