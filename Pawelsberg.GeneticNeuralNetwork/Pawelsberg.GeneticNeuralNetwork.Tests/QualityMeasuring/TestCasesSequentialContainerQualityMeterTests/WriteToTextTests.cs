using System.Text;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.QualityMeasuring;
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
        TestCasesSequentialContainerQualityMeter container = new TestCasesSequentialContainerQualityMeter(0.001, TestHelper.CreateFactory());
        container.TestCaseList = testCaseList;
        container.Propagations = 10;
        StringBuilder sb = new StringBuilder();

        container.WriteToText(sb);

        string text = sb.ToString();
        Assert.StartsWith("TestCasesSequential()", text);
    }

    [Fact]
    public void WritesAfterAllTestCasesSectionWithStaticChildren()
    {
        TestCaseList testCaseList = TestHelper.CreateTestCaseList();
        TestCasesSequentialContainerQualityMeter container = new TestCasesSequentialContainerQualityMeter(0.001, TestHelper.CreateFactory());
        container.TestCaseList = testCaseList;
        container.Propagations = 10;
        TestCasesIfAllGoodNetworkQualityMeter staticChild = new TestCasesIfAllGoodNetworkQualityMeter(container, 0.001);
        container.AddStaticChild(staticChild);
        StringBuilder sb = new StringBuilder();

        container.WriteToText(sb);

        string text = sb.ToString();
        Assert.Contains("AfterAllTestCases:", text);
        Assert.Contains("FromTestCases:", text);
        Assert.Contains("IfAllGood(0.001)", text);
    }

    [Fact]
    public void WritesAfterAllTestCasesSectionWithNestedChildren()
    {
        TestCaseList testCaseList = TestHelper.CreateTestCaseList();
        TestCasesSequentialContainerQualityMeter container = new TestCasesSequentialContainerQualityMeter(0.001, TestHelper.CreateFactory());
        container.TestCaseList = testCaseList;
        container.Propagations = 10;
        
        TestCasesIfAllGoodNetworkQualityMeter ifAllGood = new TestCasesIfAllGoodNetworkQualityMeter(container, 0.001);
        ifAllGood.Children.Add(new TestCasesTotalTimeNetworkQualityMeter(ifAllGood, 25));
        ifAllGood.Children.Add(new TotalNodesNetworkQualityMeter(ifAllGood, 5));
        container.AddStaticChild(ifAllGood);
        StringBuilder sb = new StringBuilder();

        container.WriteToText(sb);

        string text = sb.ToString();
        Assert.Contains("AfterAllTestCases:", text);
        Assert.Contains("IfAllGood(0.001)", text);
        Assert.Contains("TotalTime(25)", text);
        Assert.Contains("TotalNodes(5)", text);
    }
}
