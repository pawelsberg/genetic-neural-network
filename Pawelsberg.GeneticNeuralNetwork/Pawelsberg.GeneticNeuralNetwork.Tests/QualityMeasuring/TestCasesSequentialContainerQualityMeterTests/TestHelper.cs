using System;
using System.Collections.Generic;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCasesSequentialContainerQualityMeterTests;

public static class TestHelper
{
    public static TestCaseList CreateTestCaseList()
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

    public static Func<QualityMeter<Network>, TestCase, int, TestCaseNetworkQualityMeter> CreateFactory()
    {
        return (parent, tc, props) => new TestCaseNetworkQualityMeter(parent, tc, props);
    }
}
