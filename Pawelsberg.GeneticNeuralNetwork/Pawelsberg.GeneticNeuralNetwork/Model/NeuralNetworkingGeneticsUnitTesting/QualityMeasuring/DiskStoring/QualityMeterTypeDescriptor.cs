using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.QualityMeasuring;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring.DiskStoring;

/// <summary>
/// Describes a quality meter type for serialization/deserialization.
/// Section membership is carried by the meter type itself via IPerTestCase/IFromTestCases/IFromNetwork
/// interfaces — see NetworkQualityMeterSectionExtensions.
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
    Func<CodedText, Func<string, QualityMeter<Network>, QualityMeter<Network>?>, QualityMeter<Network>> ContainerParser
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
            new QualityMeterTypeDescriptor(
                TestCaseListNetworkQualityMeter.TextName,
                (innerText, parent) => TestCaseListNetworkQualityMeter.Parse(innerText, parent)
            ),
            new QualityMeterTypeDescriptor(
                TestCasesIfAllGoodNetworkQualityMeter.TextName,
                (innerText, parent) => TestCasesIfAllGoodNetworkQualityMeter.Parse(innerText, parent)
            ),
            new QualityMeterTypeDescriptor(
                TestCaseDifferenceNetworkQualityMeter.TextName,
                (innerText, parent) => TestCaseDifferenceNetworkQualityMeter.Parse(innerText, parent)
            ),
            new QualityMeterTypeDescriptor(
                TestCaseGoodResultNetworkQualityMeter.TextName,
                (innerText, parent) => TestCaseGoodResultNetworkQualityMeter.Parse(innerText, parent)
            ),
            new QualityMeterTypeDescriptor(
                TestCaseSubstractErrorIfGoodNetworkQualityMeter.TextName,
                (innerText, parent) => TestCaseSubstractErrorIfGoodNetworkQualityMeter.Parse(innerText, parent)
            ),
            new QualityMeterTypeDescriptor(
                TestCasesTotalTimeNetworkQualityMeter.TextName,
                (innerText, parent) => TestCasesTotalTimeNetworkQualityMeter.Parse(innerText, parent)
            ),
            new QualityMeterTypeDescriptor(
                TotalNodesNetworkQualityMeter.TextName,
                (innerText, parent) => TotalNodesNetworkQualityMeter.Parse(innerText, parent)
            ),
            new QualityMeterTypeDescriptor(
                TotalSynapsesNetworkQualityMeter.TextName,
                (innerText, parent) => TotalSynapsesNetworkQualityMeter.Parse(innerText, parent)
            ),
            new QualityMeterTypeDescriptor(
                MultiplierSumNetworkQualityMeter.TextName,
                (innerText, parent) => MultiplierSumNetworkQualityMeter.Parse(innerText, parent)
            ),
            new QualityMeterTypeDescriptor(
                NoLoopsNetworkQualityMeter.TextName,
                (innerText, parent) => NoLoopsNetworkQualityMeter.Parse(innerText, parent)
            ),
            new QualityMeterTypeDescriptor(
                SequentialOutputTestCaseNetworkQualityMeter.TextName,
                (innerText, parent) => SequentialOutputTestCaseNetworkQualityMeter.Parse(innerText, parent)
            )
        };

        _byTextName = _descriptors.ToDictionary(d => d.TextName);

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
        if (_byTextName.TryGetValue(textName, out QualityMeterTypeDescriptor descriptor))
            return descriptor.Parser(parameters, parent);
        return null;
    }

    /// <summary>
    /// Try to parse a container meter from its text name.
    /// </summary>
    public static QualityMeter<Network>? TryParseContainer(
        string textName,
        CodedText text,
        Func<string, QualityMeter<Network>, QualityMeter<Network>?> singleMeterParser)
    {
        if (_containerByTextName.TryGetValue(textName, out ContainerQualityMeterTypeDescriptor descriptor))
            return descriptor.ContainerParser(text, singleMeterParser);
        return null;
    }

    /// <summary>
    /// Gets the container descriptor for a given first line of text.
    /// </summary>
    public static ContainerQualityMeterTypeDescriptor? GetContainerDescriptorForLine(string firstLine)
    {
        foreach (ContainerQualityMeterTypeDescriptor descriptor in _containerDescriptors)
        {
            // TestCasesSequential must be checked before TestCases since it starts with "TestCases"
            if (firstLine.StartsWith($"{descriptor.TextName}("))
                return descriptor;
        }
        return null;
    }
}
