using System.Collections.Generic;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCaseListNetworkQualityMeterTests;

public class ImplementsITestCasesQualityMeterContainerTests
{
    [Fact]
    public void IsAssignable()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        TestCaseList testCaseList = new TestCaseList { TestCases = new List<TestCase>() };
        TestCaseListNetworkQualityMeter meter = new TestCaseListNetworkQualityMeter(parent, testCaseList, 10);

        Assert.IsAssignableFrom<ITestCasesQualityMeterContainer>(meter);
    }
}
