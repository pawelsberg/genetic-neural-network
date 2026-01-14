using System.Collections.Generic;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCaseNetworkQualityMeterTests;

public class ConstructorTests
{
    [Fact]
    public void SetsTestCase()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        TestCase testCase = new TestCase { Inputs = new List<int> { 1 }, Outputs = new List<int> { 0 } };
        int propagations = 10;

        TestCaseNetworkQualityMeter meter = new TestCaseNetworkQualityMeter(parent, testCase, propagations);

        Assert.Same(testCase, meter.TestCase);
    }

    [Fact]
    public void SetsPropagations()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        TestCase testCase = new TestCase { Inputs = new List<int> { 1 }, Outputs = new List<int> { 0 } };
        int propagations = 10;

        TestCaseNetworkQualityMeter meter = new TestCaseNetworkQualityMeter(parent, testCase, propagations);

        Assert.Equal(propagations, meter.Propagations);
    }

    [Fact]
    public void SetsParent()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        TestCase testCase = new TestCase { Inputs = new List<int> { 1 }, Outputs = new List<int> { 0 } };
        int propagations = 10;

        TestCaseNetworkQualityMeter meter = new TestCaseNetworkQualityMeter(parent, testCase, propagations);

        Assert.Same(parent, meter.Parent);
    }
}
