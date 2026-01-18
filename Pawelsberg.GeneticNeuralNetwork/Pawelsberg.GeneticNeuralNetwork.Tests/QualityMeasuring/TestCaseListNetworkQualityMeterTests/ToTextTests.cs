using System.Collections.Generic;
using System.Globalization;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCaseListNetworkQualityMeterTests;

public class ToTextTests
{
    [Fact]
    public void FormatsCorrectly()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        TestCaseListNetworkQualityMeter meter = new TestCaseListNetworkQualityMeter(parent);

        string text = meter.ToText();

        Assert.Equal("TestCaseList()", text);
    }

    [Fact]
    public void UsesInvariantCulture()
    {
        CultureInfo originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            QualityMeter<Network> parent = new QualityMeter<Network>(null);
            TestCaseListNetworkQualityMeter meter = new TestCaseListNetworkQualityMeter(parent);

            string text = meter.ToText();

            Assert.Equal("TestCaseList()", text);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }
}
