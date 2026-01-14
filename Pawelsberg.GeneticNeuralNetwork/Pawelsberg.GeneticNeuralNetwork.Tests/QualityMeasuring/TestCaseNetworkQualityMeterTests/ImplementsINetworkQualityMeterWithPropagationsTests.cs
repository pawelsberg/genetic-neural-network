using System.Collections.Generic;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCaseNetworkQualityMeterTests;

public class ImplementsINetworkQualityMeterWithPropagationsTests
{
    [Fact]
    public void IsAssignable()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        TestCase testCase = new TestCase { Inputs = new List<int> { 1 }, Outputs = new List<int> { 0 } };
        TestCaseNetworkQualityMeter meter = new TestCaseNetworkQualityMeter(parent, testCase, 10);

        Assert.IsAssignableFrom<INetworkQualityMeterWithPropagations>(meter);
    }

    [Fact]
    public void PropagationsPropertyReturnsCorrectValue()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        TestCase testCase = new TestCase { Inputs = new List<int> { 1 }, Outputs = new List<int> { 0 } };
        TestCaseNetworkQualityMeter meter = new TestCaseNetworkQualityMeter(parent, testCase, 10);

        Assert.Equal(10, ((INetworkQualityMeterWithPropagations)meter).Propagations);
    }
}
