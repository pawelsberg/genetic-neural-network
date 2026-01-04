using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting.DiskStoring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring;

public class TestCasesContainerQualityMeterTests
{
    [Fact]
    public void TextName_IsTestCases()
    {
        Assert.Equal("TestCases", TestCasesContainerQualityMeter.TextName);
    }

    [Fact]
    public void Constructor_SinglePropagation_SetsProperties()
    {
        var testCaseList = CreateTestCaseList();
        int propagations = 10;
        Func<QualityMeter<Network>, TestCase, int, TestCaseNetworkQualityMeter> factory = 
            (parent, tc, props) => new TestCaseNetworkQualityMeter(parent, tc, props);

        var container = new TestCasesContainerQualityMeter(testCaseList, propagations, factory);

        Assert.Same(testCaseList, container.TestCaseList);
        Assert.Equal(propagations, container.Propagations);
    }

    [Fact]
    public void Constructor_PropagationRange_SetsProperties()
    {
        var testCaseList = CreateTestCaseList();
        Func<QualityMeter<Network>, TestCase, int, TestCaseNetworkQualityMeter> factory = 
            (parent, tc, props) => new TestCaseNetworkQualityMeter(parent, tc, props);

        var container = new TestCasesContainerQualityMeter(testCaseList, 5, 15, factory);

        Assert.Same(testCaseList, container.TestCaseList);
        Assert.Equal(5, container.Propagations);
        var (from, to) = container.GetPropagationRange();
        Assert.Equal(5, from);
        Assert.Equal(15, to);
    }

    [Fact]
    public void Children_CreatesTestCaseMeters()
    {
        var testCaseList = CreateTestCaseList();
        Func<QualityMeter<Network>, TestCase, int, TestCaseNetworkQualityMeter> factory = 
            (parent, tc, props) => new TestCaseNetworkQualityMeter(parent, tc, props);

        var container = new TestCasesContainerQualityMeter(testCaseList, 10, factory);

        var children = container.Children;
        Assert.Equal(2, children.Count); // 2 test cases
        Assert.All(children, c => Assert.IsType<TestCaseNetworkQualityMeter>(c));
    }

    [Fact]
    public void Children_WithPropagationRange_CreatesMetersForEachPropagation()
    {
        var testCaseList = CreateTestCaseList();
        Func<QualityMeter<Network>, TestCase, int, TestCaseNetworkQualityMeter> factory = 
            (parent, tc, props) => new TestCaseNetworkQualityMeter(parent, tc, props);

        var container = new TestCasesContainerQualityMeter(testCaseList, 5, 7, factory);

        var children = container.Children;
        // 2 test cases * 3 propagations (5, 6, 7) = 6 meters
        Assert.Equal(6, children.Count);
    }

    [Fact]
    public void AddStaticChild_AddsToStaticChildren()
    {
        var testCaseList = CreateTestCaseList();
        Func<QualityMeter<Network>, TestCase, int, TestCaseNetworkQualityMeter> factory = 
            (parent, tc, props) => new TestCaseNetworkQualityMeter(parent, tc, props);
        var container = new TestCasesContainerQualityMeter(testCaseList, 10, factory);
        var staticChild = new TestCasesIfAllGoodNetworkQualityMeter(container, 0.001);

        container.AddStaticChild(staticChild);

        var staticChildren = container.GetStaticChildren().ToList();
        Assert.Single(staticChildren);
        Assert.Same(staticChild, staticChildren[0]);
    }

    [Fact]
    public void ImplementsINetworkQualityMeterContainerTextConvertible()
    {
        var testCaseList = CreateTestCaseList();
        Func<QualityMeter<Network>, TestCase, int, TestCaseNetworkQualityMeter> factory = 
            (parent, tc, props) => new TestCaseNetworkQualityMeter(parent, tc, props);
        var container = new TestCasesContainerQualityMeter(testCaseList, 10, factory);

        Assert.IsAssignableFrom<INetworkQualityMeterContainerTextConvertible>(container);
    }

    [Fact]
    public void WriteToText_WritesTestCasesHeader()
    {
        var testCaseList = CreateTestCaseList();
        Func<QualityMeter<Network>, TestCase, int, TestCaseNetworkQualityMeter> factory = 
            (parent, tc, props) => new TestCaseNetworkQualityMeter(parent, tc, props);
        var container = new TestCasesContainerQualityMeter(testCaseList, 10, factory);
        var sb = new StringBuilder();

        container.WriteToText(sb);

        string text = sb.ToString();
        Assert.StartsWith("TestCases(10)", text);
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
