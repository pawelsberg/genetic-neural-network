using System.Text;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCasesContainerQualityMeterTests;

public class WriteToTextTests
{
    [Fact]
    public void WritesTestCasesHeader()
    {
        TestCaseList testCaseList = TestHelper.CreateTestCaseList();
        TestCasesContainerQualityMeter container = new TestCasesContainerQualityMeter(TestHelper.CreateFactory());
        container.TestCaseList = testCaseList;
        container.Propagations = 10;
        StringBuilder sb = new StringBuilder();

        container.WriteToText(sb);

        string text = sb.ToString();
        Assert.StartsWith("TestCases()", text);
    }
}
