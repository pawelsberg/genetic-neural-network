using System;
using System.Collections.Generic;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring.DiskStoring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.DiskStoring;

public class NetworkQualityMetersTextExtensionTests
{
    [Fact]
    public void ToText_TestCaseList_FormatsCorrectly()
    {
        var testCaseList = CreateTestCaseList();
        var meter = new TestCaseListNetworkQualityMeter(null, testCaseList, 10);

        string text = meter.ToText();

        Assert.Contains("TestCaseList(10)", text);
    }

    [Fact]
    public void Parse_TestCaseList_ParsesCorrectly()
    {
        var testCaseList = CreateTestCaseList();
        string text = "TestCaseList(10)";

        var meter = NetworkQualityMetersTextExtension.Parse(text, 10, testCaseList);

        Assert.IsType<TestCaseListNetworkQualityMeter>(meter);
        var tcMeter = (TestCaseListNetworkQualityMeter)meter;
        Assert.Equal(10, tcMeter.Propagations);
    }

    [Fact]
    public void Parse_TestCases_ParsesCorrectly()
    {
        var testCaseList = CreateTestCaseList();
        string text = "TestCases(10)\n  Difference(1,0.001)";

        var meter = NetworkQualityMetersTextExtension.Parse(text, 10, testCaseList);

        Assert.IsType<TestCasesContainerQualityMeter>(meter);
    }

    [Fact]
    public void Parse_TestCasesSequential_ParsesCorrectly()
    {
        var testCaseList = CreateTestCaseList();
        string text = "TestCasesSequential(10)\n  Difference(1,0.001)";

        var meter = NetworkQualityMetersTextExtension.Parse(text, 10, testCaseList);

        Assert.IsType<TestCasesSequentialContainerQualityMeter>(meter);
    }

    [Fact]
    public void Parse_TestCasesWithRange_ParsesCorrectly()
    {
        var testCaseList = CreateTestCaseList();
        string text = "TestCases(5-10)\n  Difference(1,0.001)";

        var meter = NetworkQualityMetersTextExtension.Parse(text, 10, testCaseList);

        Assert.IsType<TestCasesContainerQualityMeter>(meter);
        var container = (TestCasesContainerQualityMeter)meter;
        var (from, to) = container.GetPropagationRange();
        Assert.Equal(5, from);
        Assert.Equal(10, to);
    }

    [Fact]
    public void Parse_EmptyText_ReturnsEmptyMeter()
    {
        var testCaseList = CreateTestCaseList();
        string text = "";

        var meter = NetworkQualityMetersTextExtension.Parse(text, 10, testCaseList);

        Assert.NotNull(meter);
        Assert.Empty(meter.Children);
    }

    [Fact]
    public void ToText_AndParse_RoundTrip_TestCases()
    {
        var testCaseList = CreateTestCaseList();
        Func<QualityMeter<Network>, TestCase, int, TestCaseNetworkQualityMeter> factory = (parent, tc, props) =>
        {
            var tcMeter = new TestCaseNetworkQualityMeter(parent, tc, props);
            tcMeter.Children.Add(new TestCaseDifferenceNetworkQualityMeter(tcMeter, 0.001, 1.0));
            return tcMeter;
        };
        var original = new TestCasesContainerQualityMeter(testCaseList, 10, factory);

        string text = original.ToText();
        var parsed = NetworkQualityMetersTextExtension.Parse(text, 10, testCaseList);

        Assert.IsType<TestCasesContainerQualityMeter>(parsed);
        var parsedContainer = (TestCasesContainerQualityMeter)parsed;
        Assert.NotEmpty(parsedContainer.Children);
    }

    [Fact]
    public void ToText_AndParse_RoundTrip_TestCasesSequential()
    {
        var testCaseList = CreateTestCaseList();
        Func<QualityMeter<Network>, TestCase, int, TestCaseNetworkQualityMeter> factory = (parent, tc, props) =>
        {
            var tcMeter = new TestCaseNetworkQualityMeter(parent, tc, props);
            tcMeter.Children.Add(new TestCaseDifferenceNetworkQualityMeter(tcMeter, 0.001, 1.0));
            return tcMeter;
        };
        var original = new TestCasesSequentialContainerQualityMeter(testCaseList, 10, 0.001, factory);

        string text = original.ToText();
        var parsed = NetworkQualityMetersTextExtension.Parse(text, 10, testCaseList);

        Assert.IsType<TestCasesSequentialContainerQualityMeter>(parsed);
    }

    private static TestCaseList CreateTestCaseList()
    {
        return new TestCaseList
        {
            TestCases = new List<TestCase>
            {
                new TestCase { Inputs = new List<int> { 0 }, Outputs = new List<int> { 0 } },
                new TestCase { Inputs = new List<int> { 1 }, Outputs = new List<int> { 1 } }
            }
        };
    }
}
