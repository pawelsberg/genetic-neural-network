using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.QualityMeasuring;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring.DiskStoring;

/// <summary>
/// Describes a quality meter type for serialization/deserialization.
/// Contains the text name and parser function for each meter type.
/// </summary>
public record QualityMeterTypeDescriptor(
    string TextName,
    Func<string, QualityMeter<Network>, QualityMeter<Network>?> Parser
);

/// <summary>
/// Describes a container quality meter type that requires multi-line parsing.
/// </summary>
public record ContainerQualityMeterTypeDescriptor(
    string TextName,
    Func<CodedLines, Func<string, QualityMeter<Network>, QualityMeter<Network>?>, QualityMeter<Network>> ContainerParser
);

/// <summary>
/// Registry of all quality meter types with their parsing functions.
/// </summary>
public static class QualityMeterTypeRegistry
{
    private static readonly List<QualityMeterTypeDescriptor> _descriptors;
    private static readonly Dictionary<string, QualityMeterTypeDescriptor> _byTextName;
    private static readonly List<ContainerQualityMeterTypeDescriptor> _containerDescriptors;
    private static readonly Dictionary<string, ContainerQualityMeterTypeDescriptor> _containerByTextName;

    public static IReadOnlyList<QualityMeterTypeDescriptor> Descriptors => _descriptors;
    public static IReadOnlyDictionary<string, QualityMeterTypeDescriptor> ByTextName => _byTextName;
    public static IReadOnlyList<ContainerQualityMeterTypeDescriptor> ContainerDescriptors => _containerDescriptors;
    public static IReadOnlyDictionary<string, ContainerQualityMeterTypeDescriptor> ContainerByTextName => _containerByTextName;

    static QualityMeterTypeRegistry()
    {
        _descriptors = new List<QualityMeterTypeDescriptor>
        {
            // TestCaseList (Aggregate)
            new QualityMeterTypeDescriptor(
                TestCaseListNetworkQualityMeter.TextName,
                (innerText, parent) => TestCaseListNetworkQualityMeter.Parse(innerText, parent)
            ),

            // IfAllGood
            new QualityMeterTypeDescriptor(
                TestCasesIfAllGoodNetworkQualityMeter.TextName,
                (innerText, parent) => TestCasesIfAllGoodNetworkQualityMeter.Parse(innerText, parent)
            ),

            // Difference
            new QualityMeterTypeDescriptor(
                TestCaseDifferenceNetworkQualityMeter.TextName,
                (innerText, parent) => TestCaseDifferenceNetworkQualityMeter.Parse(innerText, parent)
            ),

            // GoodResult
            new QualityMeterTypeDescriptor(
                TestCaseGoodResultNetworkQualityMeter.TextName,
                (innerText, parent) => TestCaseGoodResultNetworkQualityMeter.Parse(innerText, parent)
            ),

            // SubstractErrorIfGood
            new QualityMeterTypeDescriptor(
                TestCaseSubstractErrorIfGoodNetworkQualityMeter.TextName,
                (innerText, parent) => TestCaseSubstractErrorIfGoodNetworkQualityMeter.Parse(innerText, parent)
            ),

            // TotalTime
            new QualityMeterTypeDescriptor(
                TestCasesTotalTimeNetworkQualityMeter.TextName,
                (innerText, parent) => TestCasesTotalTimeNetworkQualityMeter.Parse(innerText, parent)
            ),

            // TotalNodes
            new QualityMeterTypeDescriptor(
                TotalNodesNetworkQualityMeter.TextName,
                (innerText, parent) => TotalNodesNetworkQualityMeter.Parse(innerText, parent)
            ),

            // TotalSynapses
            new QualityMeterTypeDescriptor(
                TotalSynapsesNetworkQualityMeter.TextName,
                (innerText, parent) => TotalSynapsesNetworkQualityMeter.Parse(innerText, parent)
            ),

            // MultiplierSum
            new QualityMeterTypeDescriptor(
                MultiplierSumNetworkQualityMeter.TextName,
                (innerText, parent) => MultiplierSumNetworkQualityMeter.Parse(innerText, parent)
            ),

            // NoLoops
            new QualityMeterTypeDescriptor(
                NoLoopsNetworkQualityMeter.TextName,
                (innerText, parent) => NoLoopsNetworkQualityMeter.Parse(innerText, parent)
            ),

            // SequentialOutputTestCase
            new QualityMeterTypeDescriptor(
                SequentialOutputTestCaseNetworkQualityMeter.TextName,
                (innerText, parent) => SequentialOutputTestCaseNetworkQualityMeter.Parse(innerText, parent)
            )
        };

        _byTextName = _descriptors.ToDictionary(d => d.TextName);

        // Container types require multi-line parsing
        _containerDescriptors = new List<ContainerQualityMeterTypeDescriptor>
        {
            new ContainerQualityMeterTypeDescriptor(
                TestCasesSequentialContainerQualityMeter.TextName,
                TestCasesSequentialContainerQualityMeter.Parse
            ),
            new ContainerQualityMeterTypeDescriptor(
                TestCasesContainerQualityMeter.TextName,
                TestCasesContainerQualityMeter.Parse
            )
        };

        _containerByTextName = _containerDescriptors.ToDictionary(d => d.TextName);
    }

    /// <summary>
    /// Try to parse a meter from its text name and parameters.
    /// </summary>
    public static QualityMeter<Network>? TryParse(
        string textName,
        string parameters,
        QualityMeter<Network> parent)
    {
        if (_byTextName.TryGetValue(textName, out var descriptor))
            return descriptor.Parser(parameters, parent);
        return null;
    }

    /// <summary>
    /// Try to parse a container meter from its text name.
    /// </summary>
    public static QualityMeter<Network>? TryParseContainer(
        string textName,
        CodedLines lines,
        Func<string, QualityMeter<Network>, QualityMeter<Network>?> singleMeterParser)
    {
        if (_containerByTextName.TryGetValue(textName, out var descriptor))
            return descriptor.ContainerParser(lines, singleMeterParser);
        return null;
    }

    /// <summary>
    /// Gets the container descriptor for a given first line of text.
    /// </summary>
    public static ContainerQualityMeterTypeDescriptor? GetContainerDescriptorForLine(string firstLine)
    {
        foreach (var descriptor in _containerDescriptors)
        {
            // TestCasesSequential must be checked before TestCases since it starts with "TestCases"
            if (firstLine.StartsWith($"{descriptor.TextName}("))
                return descriptor;
        }
        return null;
    }
}
