using System;
using System.Collections.Generic;
using System.Globalization;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring;

public class TestCaseNetworkQualityMeterTests
{
    [Fact]
    public void TextName_IsTestCase()
    {
        Assert.Equal("TestCase", TestCaseNetworkQualityMeter.TextName);
    }

    [Fact]
    public void Constructor_SetsProperties()
    {
        var parent = new QualityMeter<Network>(null);
        var testCase = new TestCase { Inputs = new List<int> { 1 }, Outputs = new List<int> { 0 } };
        int propagations = 10;

        var meter = new TestCaseNetworkQualityMeter(parent, testCase, propagations);

        Assert.Same(testCase, meter.TestCase);
        Assert.Equal(propagations, meter.Propagations);
        Assert.Same(parent, meter.Parent);
    }

    [Fact]
    public void ToText_FormatsCorrectly()
    {
        var parent = new QualityMeter<Network>(null);
        var testCase = new TestCase { Inputs = new List<int> { 1 }, Outputs = new List<int> { 0 } };
        var meter = new TestCaseNetworkQualityMeter(parent, testCase, 10);

        string text = meter.ToText();

        Assert.Equal("TestCase(10)", text);
    }

    [Fact]
    public void ToText_UsesInvariantCulture()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            var parent = new QualityMeter<Network>(null);
            var testCase = new TestCase { Inputs = new List<int> { 1 }, Outputs = new List<int> { 0 } };
            var meter = new TestCaseNetworkQualityMeter(parent, testCase, 25);

            string text = meter.ToText();

            Assert.Equal("TestCase(25)", text);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    [Fact]
    public void ImplementsINetworkQualityMeterWithPropagations()
    {
        var parent = new QualityMeter<Network>(null);
        var testCase = new TestCase { Inputs = new List<int> { 1 }, Outputs = new List<int> { 0 } };
        var meter = new TestCaseNetworkQualityMeter(parent, testCase, 10);

        Assert.IsAssignableFrom<INetworkQualityMeterWithPropagations>(meter);
        Assert.Equal(10, ((INetworkQualityMeterWithPropagations)meter).Propagations);
    }
}
