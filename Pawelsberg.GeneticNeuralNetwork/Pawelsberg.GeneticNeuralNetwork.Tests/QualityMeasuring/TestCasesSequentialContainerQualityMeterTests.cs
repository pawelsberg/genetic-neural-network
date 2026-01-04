using System;
using System.Collections.Generic;
using System.Text;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring;

public class TestCasesSequentialContainerQualityMeterTests
{
    [Fact]
    public void TextName_IsTestCasesSequential()
    {
        Assert.Equal("TestCasesSequential", TestCasesSequentialContainerQualityMeter.TextName);
    }

    [Fact]
    public void Constructor_SetsProperties()
    {
        var testCaseList = CreateTestCaseList();
        int propagations = 10;
        double goodDifference = 0.001;
        Func<QualityMeter<Network>, TestCase, int, TestCaseNetworkQualityMeter> factory = 
            (parent, tc, props) => new TestCaseNetworkQualityMeter(parent, tc, props);

        var container = new TestCasesSequentialContainerQualityMeter(testCaseList, propagations, goodDifference, factory);

        Assert.Same(testCaseList, container.TestCaseList);
        Assert.Equal(propagations, container.Propagations);
    }

    [Fact]
    public void Children_CreatesSequentialStructure()
    {
        var testCaseList = CreateTestCaseList();
        Func<QualityMeter<Network>, TestCase, int, TestCaseNetworkQualityMeter> factory = 
            (parent, tc, props) => new TestCaseNetworkQualityMeter(parent, tc, props);

        var container = new TestCasesSequentialContainerQualityMeter(testCaseList, 10, 0.001, factory);

        var children = container.Children;
        // Structure: TestCase1, IfAllGood1
        Assert.Equal(2, children.Count);
        Assert.IsType<TestCaseNetworkQualityMeter>(children[0]);
        Assert.IsAssignableFrom<TestCasesIfAllGoodNetworkQualityMeter>(children[1]);
    }

    [Fact]
    public void ImplementsINetworkQualityMeterContainerTextConvertible()
    {
        var testCaseList = CreateTestCaseList();
        Func<QualityMeter<Network>, TestCase, int, TestCaseNetworkQualityMeter> factory = 
            (parent, tc, props) => new TestCaseNetworkQualityMeter(parent, tc, props);
        var container = new TestCasesSequentialContainerQualityMeter(testCaseList, 10, 0.001, factory);

        Assert.IsAssignableFrom<INetworkQualityMeterContainerTextConvertible>(container);
    }

    [Fact]
    public void WriteToText_WritesTestCasesSequentialHeader()
    {
        var testCaseList = CreateTestCaseList();
        Func<QualityMeter<Network>, TestCase, int, TestCaseNetworkQualityMeter> factory = 
            (parent, tc, props) => new TestCaseNetworkQualityMeter(parent, tc, props);
        var container = new TestCasesSequentialContainerQualityMeter(testCaseList, 10, 0.001, factory);
        var sb = new StringBuilder();

        container.WriteToText(sb);

        string text = sb.ToString();
        Assert.StartsWith("TestCasesSequential(10)", text);
    }

    [Fact]
    public void ImplementsITestCasesQualityMeterContainer()
    {
        var testCaseList = CreateTestCaseList();
        Func<QualityMeter<Network>, TestCase, int, TestCaseNetworkQualityMeter> factory = 
            (parent, tc, props) => new TestCaseNetworkQualityMeter(parent, tc, props);
        var container = new TestCasesSequentialContainerQualityMeter(testCaseList, 10, 0.001, factory);

        Assert.IsAssignableFrom<ITestCasesQualityMeterContainer>(container);
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
