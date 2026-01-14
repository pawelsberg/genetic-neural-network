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
        TestCaseList testCaseList = new TestCaseList { TestCases = new List<TestCase>() };
        TestCaseListNetworkQualityMeter meter = new TestCaseListNetworkQualityMeter(parent, testCaseList, 10);

        string text = meter.ToText();

        Assert.Equal("TestCaseList(10)", text);
    }

    [Fact]
    public void UsesInvariantCulture()
    {
        CultureInfo originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            QualityMeter<Network> parent = new QualityMeter<Network>(null);
            TestCaseList testCaseList = new TestCaseList { TestCases = new List<TestCase>() };
            TestCaseListNetworkQualityMeter meter = new TestCaseListNetworkQualityMeter(parent, testCaseList, 25);

            string text = meter.ToText();

            Assert.Equal("TestCaseList(25)", text);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }
}
