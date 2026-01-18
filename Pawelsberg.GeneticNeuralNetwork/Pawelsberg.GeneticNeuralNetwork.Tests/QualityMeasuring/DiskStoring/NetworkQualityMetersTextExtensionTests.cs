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
        TestCaseListNetworkQualityMeter meter = new TestCaseListNetworkQualityMeter(null);

        string text = meter.ToText();

        Assert.Contains("TestCaseList()", text);
    }

    [Fact]
    public void Parse_TestCaseList_ParsesCorrectly()
    {
        string text = "TestCaseList()";

        QualityMeter<Network> meter = NetworkQualityMetersTextExtension.Parse(text);

        Assert.IsType<TestCaseListNetworkQualityMeter>(meter);
    }

    [Fact]
    public void Parse_TestCases_ParsesCorrectly()
    {
        string text = "TestCases()\n  Difference(1,0.001)";

        QualityMeter<Network> meter = NetworkQualityMetersTextExtension.Parse(text);

        Assert.IsType<TestCasesContainerQualityMeter>(meter);
    }

    [Fact]
    public void Parse_TestCasesSequential_ParsesCorrectly()
    {
        string text = "TestCasesSequential()\n  Difference(1,0.001)";

        QualityMeter<Network> meter = NetworkQualityMetersTextExtension.Parse(text);

        Assert.IsType<TestCasesSequentialContainerQualityMeter>(meter);
    }

    [Fact]
    public void Parse_TestCasesWithAdditionalPropagations_ParsesCorrectly()
    {
        TestCaseList testCaseList = CreateTestCaseList();
        string text = "TestCases(5)\n  Difference(1,0.001)";

        QualityMeter<Network> meter = NetworkQualityMetersTextExtension.Parse(text);

        Assert.IsType<TestCasesContainerQualityMeter>(meter);
        TestCasesContainerQualityMeter container = (TestCasesContainerQualityMeter)meter;
        container.TestCaseList = testCaseList;
        container.Propagations = 10;
        (int from, int to) = container.GetPropagationRange()!.Value;
        Assert.Equal(10, from);
        Assert.Equal(15, to);
    }

    [Fact]
    public void Parse_EmptyText_ReturnsEmptyMeter()
    {
        string text = "";

        QualityMeter<Network> meter = NetworkQualityMetersTextExtension.Parse(text);

        Assert.NotNull(meter);
        Assert.Empty(meter.Children);
    }

    [Fact]
    public void ToText_AndParse_RoundTrip_TestCases()
    {
        TestCaseList testCaseList = CreateTestCaseList();
        Func<QualityMeter<Network>, TestCase, int, TestCaseNetworkQualityMeter> factory = (parent, tc, props) =>
        {
            TestCaseNetworkQualityMeter tcMeter = new TestCaseNetworkQualityMeter(parent, tc, props);
            tcMeter.Children.Add(new TestCaseDifferenceNetworkQualityMeter(tcMeter, 0.001, 1.0));
            return tcMeter;
        };
        TestCasesContainerQualityMeter original = new TestCasesContainerQualityMeter(factory);
        original.TestCaseList = testCaseList;
        original.Propagations = 10;

        string text = original.ToText();
        QualityMeter<Network> parsed = NetworkQualityMetersTextExtension.Parse(text);

        Assert.IsType<TestCasesContainerQualityMeter>(parsed);
        TestCasesContainerQualityMeter parsedContainer = (TestCasesContainerQualityMeter)parsed;
        parsedContainer.TestCaseList = testCaseList;
        parsedContainer.Propagations = 10;
        Assert.NotEmpty(parsedContainer.Children);
    }

    [Fact]
    public void ToText_AndParse_RoundTrip_TestCasesSequential()
    {
        TestCaseList testCaseList = CreateTestCaseList();
        Func<QualityMeter<Network>, TestCase, int, TestCaseNetworkQualityMeter> factory = (parent, tc, props) =>
        {
            TestCaseNetworkQualityMeter tcMeter = new TestCaseNetworkQualityMeter(parent, tc, props);
            tcMeter.Children.Add(new TestCaseDifferenceNetworkQualityMeter(tcMeter, 0.001, 1.0));
            return tcMeter;
        };
        TestCasesSequentialContainerQualityMeter original = new TestCasesSequentialContainerQualityMeter(0.001, factory);
        original.TestCaseList = testCaseList;
        original.Propagations = 10;

        string text = original.ToText();
        QualityMeter<Network> parsed = NetworkQualityMetersTextExtension.Parse(text);

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
