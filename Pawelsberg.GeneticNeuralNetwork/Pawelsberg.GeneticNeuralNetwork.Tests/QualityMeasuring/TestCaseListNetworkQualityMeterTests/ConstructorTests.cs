using System.Collections.Generic;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCaseListNetworkQualityMeterTests;

public class ConstructorTests
{
    [Fact]
    public void SetsTestCaseList()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        TestCaseList testCaseList = new TestCaseList { TestCases = new List<TestCase>() };
        int propagations = 10;

        TestCaseListNetworkQualityMeter meter = new TestCaseListNetworkQualityMeter(parent, testCaseList, propagations);

        Assert.Same(testCaseList, meter.TestCaseList);
    }

    [Fact]
    public void SetsPropagations()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        TestCaseList testCaseList = new TestCaseList { TestCases = new List<TestCase>() };
        int propagations = 10;

        TestCaseListNetworkQualityMeter meter = new TestCaseListNetworkQualityMeter(parent, testCaseList, propagations);

        Assert.Equal(propagations, meter.Propagations);
    }

    [Fact]
    public void SetsParent()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        TestCaseList testCaseList = new TestCaseList { TestCases = new List<TestCase>() };
        int propagations = 10;

        TestCaseListNetworkQualityMeter meter = new TestCaseListNetworkQualityMeter(parent, testCaseList, propagations);

        Assert.Same(parent, meter.Parent);
    }
}
