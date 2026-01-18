using System;
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
    public void SetsTestCaseListViaProperty()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        TestCaseList testCaseList = new TestCaseList { TestCases = new List<TestCase>() };

        TestCaseListNetworkQualityMeter meter = new TestCaseListNetworkQualityMeter(parent);
        meter.TestCaseList = testCaseList;

        Assert.Same(testCaseList, meter.TestCaseList);
    }

    [Fact]
    public void SetsPropagationsViaProperty()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        int propagations = 10;

        TestCaseListNetworkQualityMeter meter = new TestCaseListNetworkQualityMeter(parent);
        meter.Propagations = propagations;

        Assert.Equal(propagations, meter.Propagations);
    }

    [Fact]
    public void SetsParent()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);

        TestCaseListNetworkQualityMeter meter = new TestCaseListNetworkQualityMeter(parent);

        Assert.Same(parent, meter.Parent);
    }

    [Fact]
    public void TestCaseListReturnsNullWhenNotSet()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);

        TestCaseListNetworkQualityMeter meter = new TestCaseListNetworkQualityMeter(parent);

        Assert.Null(meter.TestCaseList);
    }

    [Fact]
    public void PropagationsReturnsNullWhenNotSet()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);

        TestCaseListNetworkQualityMeter meter = new TestCaseListNetworkQualityMeter(parent);

        Assert.Null(meter.Propagations);
    }
}
