using System.Collections.Generic;
using System.Globalization;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring;

public class TestCaseListNetworkQualityMeterTests
{
    [Fact]
    public void TextName_IsTestCaseList()
    {
        Assert.Equal("TestCaseList", TestCaseListNetworkQualityMeter.TextName);
    }

    [Fact]
    public void Constructor_SetsProperties()
    {
        var parent = new QualityMeter<Network>(null);
        var testCaseList = new TestCaseList { TestCases = new List<TestCase>() };
        int propagations = 10;

        var meter = new TestCaseListNetworkQualityMeter(parent, testCaseList, propagations);

        Assert.Same(testCaseList, meter.TestCaseList);
        Assert.Equal(propagations, meter.Propagations);
        Assert.Same(parent, meter.Parent);
    }

    [Fact]
    public void ToText_FormatsCorrectly()
    {
        var parent = new QualityMeter<Network>(null);
        var testCaseList = new TestCaseList { TestCases = new List<TestCase>() };
        var meter = new TestCaseListNetworkQualityMeter(parent, testCaseList, 10);

        string text = meter.ToText();

        Assert.Equal("TestCaseList(10)", text);
    }

    [Fact]
    public void ToText_UsesInvariantCulture()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            var parent = new QualityMeter<Network>(null);
            var testCaseList = new TestCaseList { TestCases = new List<TestCase>() };
            var meter = new TestCaseListNetworkQualityMeter(parent, testCaseList, 25);

            string text = meter.ToText();

            Assert.Equal("TestCaseList(25)", text);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    [Fact]
    public void ImplementsITestCasesQualityMeterContainer()
    {
        var parent = new QualityMeter<Network>(null);
        var testCaseList = new TestCaseList { TestCases = new List<TestCase>() };
        var meter = new TestCaseListNetworkQualityMeter(parent, testCaseList, 10);

        Assert.IsAssignableFrom<ITestCasesQualityMeterContainer>(meter);
    }
}
