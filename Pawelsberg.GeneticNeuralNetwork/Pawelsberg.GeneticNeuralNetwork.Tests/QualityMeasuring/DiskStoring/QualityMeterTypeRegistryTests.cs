using System;
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
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        string parameters = "1.0,0.001";

        QualityMeter<Network> meter = QualityMeterTypeRegistry.TryParse("Difference", parameters, parent);

        Assert.NotNull(meter);
        Assert.IsType<TestCaseDifferenceNetworkQualityMeter>(meter);
        TestCaseDifferenceNetworkQualityMeter diffMeter = (TestCaseDifferenceNetworkQualityMeter)meter;
        Assert.Equal(1.0, diffMeter.QualityForOneDiff);
        Assert.Equal(0.001, diffMeter.GoodDifference);
    }

    [Fact]
    public void TryParse_GoodResult_ParsesCorrectly()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        string parameters = "10.0,0.001";

        QualityMeter<Network> meter = QualityMeterTypeRegistry.TryParse("GoodResult", parameters, parent);

        Assert.NotNull(meter);
        Assert.IsType<TestCaseGoodResultNetworkQualityMeter>(meter);
        TestCaseGoodResultNetworkQualityMeter goodMeter = (TestCaseGoodResultNetworkQualityMeter)meter;
        Assert.Equal(10.0, goodMeter.QualityForGoodResult);
        Assert.Equal(0.001, goodMeter.GoodDifference);
    }

    [Fact]
    public void TryParse_IfAllGood_ParsesCorrectly()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        string parameters = "0.001";

        QualityMeter<Network> meter = QualityMeterTypeRegistry.TryParse("IfAllGood", parameters, parent);

        Assert.NotNull(meter);
        Assert.IsType<TestCasesIfAllGoodNetworkQualityMeter>(meter);
        TestCasesIfAllGoodNetworkQualityMeter ifAllGood = (TestCasesIfAllGoodNetworkQualityMeter)meter;
        Assert.Equal(0.001, ifAllGood.GoodDifference);
    }

    [Fact]
    public void TryParse_TotalNodes_ParsesCorrectly()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        string parameters = "10.0";

        QualityMeter<Network> meter = QualityMeterTypeRegistry.TryParse("TotalNodes", parameters, parent);

        Assert.NotNull(meter);
        Assert.IsType<TotalNodesNetworkQualityMeter>(meter);
        TotalNodesNetworkQualityMeter nodeMeter = (TotalNodesNetworkQualityMeter)meter;
        Assert.Equal(10.0, nodeMeter.QualityForOneNode);
    }

    [Fact]
    public void TryParse_TotalSynapses_ParsesCorrectly()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        string parameters = "5.0";

        QualityMeter<Network> meter = QualityMeterTypeRegistry.TryParse("TotalSynapses", parameters, parent);

        Assert.NotNull(meter);
        Assert.IsType<TotalSynapsesNetworkQualityMeter>(meter);
        TotalSynapsesNetworkQualityMeter synapseMeter = (TotalSynapsesNetworkQualityMeter)meter;
        Assert.Equal(5.0, synapseMeter.QualityForOneSynapse);
    }

    [Fact]
    public void TryParse_MultiplierSum_ParsesCorrectly()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        string parameters = "20.0";

        QualityMeter<Network> meter = QualityMeterTypeRegistry.TryParse("MultiplierSum", parameters, parent);

        Assert.NotNull(meter);
        Assert.IsType<MultiplierSumNetworkQualityMeter>(meter);
        MultiplierSumNetworkQualityMeter multMeter = (MultiplierSumNetworkQualityMeter)meter;
        Assert.Equal(20.0, multMeter.QualityForSumEqOne);
    }

    [Fact]
    public void TryParse_NoLoops_ParsesCorrectly()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        string parameters = "50.0";

        QualityMeter<Network> meter = QualityMeterTypeRegistry.TryParse("NoLoops", parameters, parent);

        Assert.NotNull(meter);
        Assert.IsType<NoLoopsNetworkQualityMeter>(meter);
        NoLoopsNetworkQualityMeter loopMeter = (NoLoopsNetworkQualityMeter)meter;
        Assert.Equal(50.0, loopMeter.QualityForZeroLoops);
    }

    [Fact]
    public void TryParse_TestCaseList_ParsesCorrectly()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        string parameters = "";

        QualityMeter<Network> meter = QualityMeterTypeRegistry.TryParse("TestCaseList", parameters, parent);

        Assert.NotNull(meter);
        Assert.IsType<TestCaseListNetworkQualityMeter>(meter);
    }

    [Fact]
    public void TryParse_UnknownType_ReturnsNull()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);

        QualityMeter<Network> meter = QualityMeterTypeRegistry.TryParse("UnknownType", "parameters", parent);

        Assert.Null(meter);
    }

    [Fact]
    public void GetContainerDescriptorForLine_TestCases_ReturnsDescriptor()
    {
        string line = "TestCases(10)";

        ContainerQualityMeterTypeDescriptor descriptor = QualityMeterTypeRegistry.GetContainerDescriptorForLine(line);

        Assert.NotNull(descriptor);
        Assert.Equal("TestCases", descriptor.TextName);
    }

    [Fact]
    public void GetContainerDescriptorForLine_TestCasesSequential_ReturnsDescriptor()
    {
        string line = "TestCasesSequential(10)";

        ContainerQualityMeterTypeDescriptor descriptor = QualityMeterTypeRegistry.GetContainerDescriptorForLine(line);

        Assert.NotNull(descriptor);
        Assert.Equal("TestCasesSequential", descriptor.TextName);
    }

    [Fact]
    public void GetContainerDescriptorForLine_NonContainer_ReturnsNull()
    {
        string line = "Difference(1,0.001)";

        ContainerQualityMeterTypeDescriptor descriptor = QualityMeterTypeRegistry.GetContainerDescriptorForLine(line);

        Assert.Null(descriptor);
    }

    [Fact]
    public void PerTestCaseMeters_ImplementPerTestCaseInterface()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        Assert.IsAssignableFrom<IPerTestCaseNetworkQualityMeter>(QualityMeterTypeRegistry.TryParse("Difference", "1,0.001", parent));
        Assert.IsAssignableFrom<IPerTestCaseNetworkQualityMeter>(QualityMeterTypeRegistry.TryParse("GoodResult", "10,0.001", parent));
        Assert.IsAssignableFrom<IPerTestCaseNetworkQualityMeter>(QualityMeterTypeRegistry.TryParse("SubstractErrorIfGood", "1,0.001", parent));
    }

    [Fact]
    public void FromTestCasesMeters_ImplementFromTestCasesInterface()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        Assert.IsAssignableFrom<IFromTestCasesNetworkQualityMeter>(QualityMeterTypeRegistry.TryParse("TotalTime", "25", parent));
        // IfAllGood appears in both FromTestCases and FromNetwork sections.
        Assert.IsAssignableFrom<IFromTestCasesNetworkQualityMeter>(QualityMeterTypeRegistry.TryParse("IfAllGood", "0.001", parent));
        Assert.IsAssignableFrom<IFromNetworkNetworkQualityMeter>(QualityMeterTypeRegistry.TryParse("IfAllGood", "0.001", parent));
    }

    [Fact]
    public void FromNetworkMeters_ImplementFromNetworkInterface()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        Assert.IsAssignableFrom<IFromNetworkNetworkQualityMeter>(QualityMeterTypeRegistry.TryParse("TotalNodes", "10", parent));
        Assert.IsAssignableFrom<IFromNetworkNetworkQualityMeter>(QualityMeterTypeRegistry.TryParse("TotalSynapses", "5", parent));
        Assert.IsAssignableFrom<IFromNetworkNetworkQualityMeter>(QualityMeterTypeRegistry.TryParse("MultiplierSum", "20", parent));
        Assert.IsAssignableFrom<IFromNetworkNetworkQualityMeter>(QualityMeterTypeRegistry.TryParse("NoLoops", "50", parent));
    }

    [Fact]
    public void GetSectionHeader_PerTestCaseMeter_ReturnsPerTestCase()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        QualityMeter<Network> meter = QualityMeterTypeRegistry.TryParse("Difference", "1,0.001", parent);

        Assert.Equal(NetworkQualityMeterSectionExtensions.PerTestCaseSection, meter.GetSectionHeader());
    }

    [Fact]
    public void GetSectionHeader_FromTestCasesMeter_ReturnsFromTestCases()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        QualityMeter<Network> meter = QualityMeterTypeRegistry.TryParse("TotalTime", "25", parent);

        Assert.Equal(NetworkQualityMeterSectionExtensions.FromTestCasesSection, meter.GetSectionHeader());
    }

    [Fact]
    public void GetSectionHeader_IfAllGood_ReturnsFromTestCasesAsPrimary()
    {
        // IfAllGood implements both IFromTestCases and IFromNetwork; primary is FromTestCases.
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        QualityMeter<Network> meter = QualityMeterTypeRegistry.TryParse("IfAllGood", "0.001", parent);

        Assert.Equal(NetworkQualityMeterSectionExtensions.FromTestCasesSection, meter.GetSectionHeader());
    }

    [Fact]
    public void GetSectionHeader_FromNetworkMeter_ReturnsFromNetwork()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        QualityMeter<Network> meter = QualityMeterTypeRegistry.TryParse("TotalNodes", "10", parent);

        Assert.Equal(NetworkQualityMeterSectionExtensions.FromNetworkSection, meter.GetSectionHeader());
    }

    [Fact]
    public void ValidateInSection_CorrectSection_DoesNotThrow()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        QualityMeterTypeRegistry.TryParse("Difference", "1,0.001", parent).ValidateInSection(NetworkQualityMeterSectionExtensions.PerTestCaseSection);
        QualityMeterTypeRegistry.TryParse("IfAllGood", "0.001", parent).ValidateInSection(NetworkQualityMeterSectionExtensions.FromTestCasesSection);
        QualityMeterTypeRegistry.TryParse("TotalNodes", "10", parent).ValidateInSection(NetworkQualityMeterSectionExtensions.FromNetworkSection);
    }

    [Fact]
    public void ValidateInSection_IfAllGoodInFromNetwork_DoesNotThrow()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        QualityMeterTypeRegistry.TryParse("IfAllGood", "0.001", parent).ValidateInSection(NetworkQualityMeterSectionExtensions.FromNetworkSection);
    }

    [Fact]
    public void ValidateInSection_WrongSection_ThrowsInvalidOperationException()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        QualityMeter<Network> meter = QualityMeterTypeRegistry.TryParse("Difference", "1,0.001", parent);

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            meter.ValidateInSection(NetworkQualityMeterSectionExtensions.FromNetworkSection));

        Assert.Contains("FromNetwork", exception.Message);
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
