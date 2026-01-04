using System.Collections.Generic;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.QualityMeasuring;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring.DiskStoring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.DiskStoring;

public class QualityMeterTypeRegistryTests
{
    [Fact]
    public void ByTextName_ContainsAllMeterTypes()
    {
        Assert.True(QualityMeterTypeRegistry.ByTextName.ContainsKey("TestCaseList"));
        Assert.True(QualityMeterTypeRegistry.ByTextName.ContainsKey("IfAllGood"));
        Assert.True(QualityMeterTypeRegistry.ByTextName.ContainsKey("Difference"));
        Assert.True(QualityMeterTypeRegistry.ByTextName.ContainsKey("GoodResult"));
        Assert.True(QualityMeterTypeRegistry.ByTextName.ContainsKey("SubstractErrorIfGood"));
        Assert.True(QualityMeterTypeRegistry.ByTextName.ContainsKey("TotalTime"));
        Assert.True(QualityMeterTypeRegistry.ByTextName.ContainsKey("TotalNodes"));
        Assert.True(QualityMeterTypeRegistry.ByTextName.ContainsKey("TotalSynapses"));
        Assert.True(QualityMeterTypeRegistry.ByTextName.ContainsKey("MultiplierSum"));
        Assert.True(QualityMeterTypeRegistry.ByTextName.ContainsKey("NoLoops"));
    }

    [Fact]
    public void ContainerByTextName_ContainsContainerTypes()
    {
        Assert.True(QualityMeterTypeRegistry.ContainerByTextName.ContainsKey("TestCases"));
        Assert.True(QualityMeterTypeRegistry.ContainerByTextName.ContainsKey("TestCasesSequential"));
    }

    [Fact]
    public void TryParse_Difference_ParsesCorrectly()
    {
        var parent = new QualityMeter<Network>(null);
        string parameters = "1.0,0.001";

        var meter = QualityMeterTypeRegistry.TryParse("Difference", parameters, parent, 10, null);

        Assert.NotNull(meter);
        Assert.IsType<TestCaseDifferenceNetworkQualityMeter>(meter);
        var diffMeter = (TestCaseDifferenceNetworkQualityMeter)meter;
        Assert.Equal(1.0, diffMeter.QualityForOneDiff);
        Assert.Equal(0.001, diffMeter.GoodDifference);
    }

    [Fact]
    public void TryParse_GoodResult_ParsesCorrectly()
    {
        var parent = new QualityMeter<Network>(null);
        string parameters = "10.0,0.001";

        var meter = QualityMeterTypeRegistry.TryParse("GoodResult", parameters, parent, 10, null);

        Assert.NotNull(meter);
        Assert.IsType<TestCaseGoodResultNetworkQualityMeter>(meter);
        var goodMeter = (TestCaseGoodResultNetworkQualityMeter)meter;
        Assert.Equal(10.0, goodMeter.QualityForGoodResult);
        Assert.Equal(0.001, goodMeter.GoodDifference);
    }

    [Fact]
    public void TryParse_IfAllGood_ParsesCorrectly()
    {
        var parent = new QualityMeter<Network>(null);
        string parameters = "0.001";

        var meter = QualityMeterTypeRegistry.TryParse("IfAllGood", parameters, parent, 10, null);

        Assert.NotNull(meter);
        Assert.IsType<TestCasesIfAllGoodNetworkQualityMeter>(meter);
        var ifAllGood = (TestCasesIfAllGoodNetworkQualityMeter)meter;
        Assert.Equal(0.001, ifAllGood.GoodDifference);
    }

    [Fact]
    public void TryParse_TotalNodes_ParsesCorrectly()
    {
        var parent = new QualityMeter<Network>(null);
        string parameters = "10.0";

        var meter = QualityMeterTypeRegistry.TryParse("TotalNodes", parameters, parent, 10, null);

        Assert.NotNull(meter);
        Assert.IsType<TotalNodesNetworkQualityMeter>(meter);
        var nodeMeter = (TotalNodesNetworkQualityMeter)meter;
        Assert.Equal(10.0, nodeMeter.QualityForOneNode);
    }

    [Fact]
    public void TryParse_TotalSynapses_ParsesCorrectly()
    {
        var parent = new QualityMeter<Network>(null);
        string parameters = "5.0";

        var meter = QualityMeterTypeRegistry.TryParse("TotalSynapses", parameters, parent, 10, null);

        Assert.NotNull(meter);
        Assert.IsType<TotalSynapsesNetworkQualityMeter>(meter);
        var synapseMeter = (TotalSynapsesNetworkQualityMeter)meter;
        Assert.Equal(5.0, synapseMeter.QualityForOneSynapse);
    }

    [Fact]
    public void TryParse_MultiplierSum_ParsesCorrectly()
    {
        var parent = new QualityMeter<Network>(null);
        string parameters = "20.0";

        var meter = QualityMeterTypeRegistry.TryParse("MultiplierSum", parameters, parent, 10, null);

        Assert.NotNull(meter);
        Assert.IsType<MultiplierSumNetworkQualityMeter>(meter);
        var multMeter = (MultiplierSumNetworkQualityMeter)meter;
        Assert.Equal(20.0, multMeter.QualityForSumEqOne);
    }

    [Fact]
    public void TryParse_NoLoops_ParsesCorrectly()
    {
        var parent = new QualityMeter<Network>(null);
        string parameters = "50.0";

        var meter = QualityMeterTypeRegistry.TryParse("NoLoops", parameters, parent, 10, null);

        Assert.NotNull(meter);
        Assert.IsType<NoLoopsNetworkQualityMeter>(meter);
        var loopMeter = (NoLoopsNetworkQualityMeter)meter;
        Assert.Equal(50.0, loopMeter.QualityForZeroLoops);
    }

    [Fact]
    public void TryParse_TestCaseList_ParsesCorrectly()
    {
        var parent = new QualityMeter<Network>(null);
        var testCaseList = CreateTestCaseList();
        string parameters = "10";

        var meter = QualityMeterTypeRegistry.TryParse("TestCaseList", parameters, parent, 10, testCaseList);

        Assert.NotNull(meter);
        Assert.IsType<TestCaseListNetworkQualityMeter>(meter);
        var tcListMeter = (TestCaseListNetworkQualityMeter)meter;
        Assert.Equal(10, tcListMeter.Propagations);
    }

    [Fact]
    public void TryParse_UnknownType_ReturnsNull()
    {
        var parent = new QualityMeter<Network>(null);

        var meter = QualityMeterTypeRegistry.TryParse("UnknownType", "parameters", parent, 10, null);

        Assert.Null(meter);
    }

    [Fact]
    public void GetContainerDescriptorForLine_TestCases_ReturnsDescriptor()
    {
        string line = "TestCases(10)";

        var descriptor = QualityMeterTypeRegistry.GetContainerDescriptorForLine(line);

        Assert.NotNull(descriptor);
        Assert.Equal("TestCases", descriptor.TextName);
    }

    [Fact]
    public void GetContainerDescriptorForLine_TestCasesSequential_ReturnsDescriptor()
    {
        string line = "TestCasesSequential(10)";

        var descriptor = QualityMeterTypeRegistry.GetContainerDescriptorForLine(line);

        Assert.NotNull(descriptor);
        Assert.Equal("TestCasesSequential", descriptor.TextName);
    }

    [Fact]
    public void GetContainerDescriptorForLine_NonContainer_ReturnsNull()
    {
        string line = "Difference(1,0.001)";

        var descriptor = QualityMeterTypeRegistry.GetContainerDescriptorForLine(line);

        Assert.Null(descriptor);
    }

    [Fact]
    public void IsPlaceholderType_TestCases_ReturnsTrue()
    {
        Assert.True(QualityMeterTypeRegistry.IsPlaceholderType("TestCases"));
    }

    [Fact]
    public void IsPlaceholderType_TestCasesSequential_ReturnsTrue()
    {
        Assert.True(QualityMeterTypeRegistry.IsPlaceholderType("TestCasesSequential"));
    }

    [Fact]
    public void IsPlaceholderType_NonPlaceholder_ReturnsFalse()
    {
        Assert.False(QualityMeterTypeRegistry.IsPlaceholderType("Difference"));
    }

    [Fact]
    public void TryParsePlaceholder_TestCases_ReturnsParsedData()
    {
        var placeholder = QualityMeterTypeRegistry.TryParsePlaceholder("TestCases", "10");

        Assert.NotNull(placeholder);
        Assert.Equal("TestCases", placeholder.TextName);
        Assert.Equal(10, placeholder.PropagationsFrom);
        Assert.Equal(10, placeholder.PropagationsTo);
    }

    [Fact]
    public void TryParsePlaceholder_TestCasesWithRange_ReturnsParsedData()
    {
        var placeholder = QualityMeterTypeRegistry.TryParsePlaceholder("TestCases", "5-15");

        Assert.NotNull(placeholder);
        Assert.Equal("TestCases", placeholder.TextName);
        Assert.Equal(5, placeholder.PropagationsFrom);
        Assert.Equal(15, placeholder.PropagationsTo);
    }

    [Fact]
    public void TestCasesPlaceholderTextName_IsTestCases()
    {
        Assert.Equal("TestCases", QualityMeterTypeRegistry.TestCasesPlaceholderTextName);
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
