using System.Collections.Generic;
using System.Globalization;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCaseNetworkQualityMeterTests;

public class ToTextTests
{
    [Fact]
    public void FormatsCorrectly()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        TestCase testCase = new TestCase { Inputs = new List<int> { 1 }, Outputs = new List<int> { 0 } };
        TestCaseNetworkQualityMeter meter = new TestCaseNetworkQualityMeter(parent, testCase, 10);

        string text = meter.ToText();

        Assert.Equal("TestCase(10)", text);
    }

    [Fact]
    public void UsesInvariantCulture()
    {
        CultureInfo originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            QualityMeter<Network> parent = new QualityMeter<Network>(null);
            TestCase testCase = new TestCase { Inputs = new List<int> { 1 }, Outputs = new List<int> { 0 } };
            TestCaseNetworkQualityMeter meter = new TestCaseNetworkQualityMeter(parent, testCase, 25);

            string text = meter.ToText();

            Assert.Equal("TestCase(25)", text);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }
}
