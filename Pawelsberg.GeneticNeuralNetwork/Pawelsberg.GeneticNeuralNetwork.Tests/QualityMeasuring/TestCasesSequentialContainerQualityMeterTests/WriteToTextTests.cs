using System.Text;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCasesSequentialContainerQualityMeterTests;

public class WriteToTextTests
{
    [Fact]
    public void WritesTestCasesSequentialHeader()
    {
        TestCaseList testCaseList = TestHelper.CreateTestCaseList();
        TestCasesSequentialContainerQualityMeter container = new TestCasesSequentialContainerQualityMeter(testCaseList, 10, 0.001, TestHelper.CreateFactory());
        StringBuilder sb = new StringBuilder();

        container.WriteToText(sb);

        string text = sb.ToString();
        Assert.StartsWith("TestCasesSequential(10)", text);
    }
}
