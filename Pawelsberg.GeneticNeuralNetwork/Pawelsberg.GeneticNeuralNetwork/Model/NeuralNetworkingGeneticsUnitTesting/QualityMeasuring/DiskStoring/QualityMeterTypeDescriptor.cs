using System.Globalization;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.QualityMeasuring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting.DiskStoring;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring.DiskStoring;

/// <summary>
/// Describes a quality meter type for serialization/deserialization.
/// Contains the text name, parser function, and parameter extraction for each meter type.
/// </summary>
public record QualityMeterTypeDescriptor(
    string TextName,
    Func<string, QualityMeter<Network>, int, TestCaseList, QualityMeter<Network>?> Parser
);

/// <summary>
/// Describes a container quality meter type that requires multi-line parsing.
/// </summary>
public record ContainerQualityMeterTypeDescriptor(
    string TextName,
    Func<CodedLines, TestCaseList, Func<string, QualityMeter<Network>, int, TestCaseList, QualityMeter<Network>?>, QualityMeter<Network>> ContainerParser
);

/// <summary>
/// Placeholder data used during static parsing to expand test case blocks.
/// </summary>
public record PlaceholderData(string TextName, int PropagationsFrom, int PropagationsTo);

/// <summary>
/// Describes a placeholder type for static parsing expansion.
/// </summary>
public record PlaceholderTypeDescriptor(
    string TextName,
    Func<string, PlaceholderData?> ParsePlaceholder,
    Action<PlaceholderData, QualityMeter<Network>, List<string>, int, TestCaseList, Func<string, QualityMeter<Network>, int, TestCaseList, QualityMeter<Network>?>> ExpandPlaceholder
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
    private static readonly List<PlaceholderTypeDescriptor> _placeholderDescriptors;
    private static readonly Dictionary<string, PlaceholderTypeDescriptor> _placeholderByTextName;

    public static IReadOnlyList<QualityMeterTypeDescriptor> Descriptors => _descriptors;
    public static IReadOnlyDictionary<string, QualityMeterTypeDescriptor> ByTextName => _byTextName;
    public static IReadOnlyList<ContainerQualityMeterTypeDescriptor> ContainerDescriptors => _containerDescriptors;
    public static IReadOnlyDictionary<string, ContainerQualityMeterTypeDescriptor> ContainerByTextName => _containerByTextName;
    public static IReadOnlyList<PlaceholderTypeDescriptor> PlaceholderDescriptors => _placeholderDescriptors;
    public static IReadOnlyDictionary<string, PlaceholderTypeDescriptor> PlaceholderByTextName => _placeholderByTextName;

    /// <summary>
    /// Gets the text name for the TestCases placeholder (used for writing static format).
    /// </summary>
    public static string TestCasesPlaceholderTextName => TestCasesContainerQualityMeter.TextName;

    static QualityMeterTypeRegistry()
    {
        _descriptors = new List<QualityMeterTypeDescriptor>
        {
            // TestCaseList (Aggregate)
            new QualityMeterTypeDescriptor(
                TestCaseListNetworkQualityMeter.TextName,
                (inner, parent, propagations, testCaseList) => TestCaseListNetworkQualityMeter.Parse(inner, parent, testCaseList)
            ),

            // IfAllGood
            new QualityMeterTypeDescriptor(
                TestCasesIfAllGoodNetworkQualityMeter.TextName,
                (inner, parent, propagations, testCaseList) => TestCasesIfAllGoodNetworkQualityMeter.Parse(inner, parent)
            ),

            // Difference
            new QualityMeterTypeDescriptor(
                TestCaseDifferenceNetworkQualityMeter.TextName,
                (inner, parent, propagations, testCaseList) => TestCaseDifferenceNetworkQualityMeter.Parse(inner, parent)
            ),

            // GoodResult
            new QualityMeterTypeDescriptor(
                TestCaseGoodResultNetworkQualityMeter.TextName,
                (inner, parent, propagations, testCaseList) => TestCaseGoodResultNetworkQualityMeter.Parse(inner, parent)
            ),

            // SubstractErrorIfGood
            new QualityMeterTypeDescriptor(
                TestCaseSubstractErrorIfGoodNetworkQualityMeter.TextName,
                (inner, parent, propagations, testCaseList) => TestCaseSubstractErrorIfGoodNetworkQualityMeter.Parse(inner, parent)
            ),

            // TotalTime
            new QualityMeterTypeDescriptor(
                TestCasesTotalTimeNetworkQualityMeter.TextName,
                (inner, parent, propagations, testCaseList) => TestCasesTotalTimeNetworkQualityMeter.Parse(inner, parent)
            ),

            // TotalNodes
            new QualityMeterTypeDescriptor(
                TotalNodesNetworkQualityMeter.TextName,
                (inner, parent, propagations, testCaseList) => TotalNodesNetworkQualityMeter.Parse(inner, parent)
            ),

            // TotalSynapses
            new QualityMeterTypeDescriptor(
                TotalSynapsesNetworkQualityMeter.TextName,
                (inner, parent, propagations, testCaseList) => TotalSynapsesNetworkQualityMeter.Parse(inner, parent)
            ),

            // MultiplierSum
            new QualityMeterTypeDescriptor(
                MultiplierSumNetworkQualityMeter.TextName,
                (inner, parent, propagations, testCaseList) => MultiplierSumNetworkQualityMeter.Parse(inner, parent)
            ),

            // NoLoops
            new QualityMeterTypeDescriptor(
                NoLoopsNetworkQualityMeter.TextName,
                (inner, parent, propagations, testCaseList) => NoLoopsNetworkQualityMeter.Parse(inner, parent)
            ),

            // SequentialOutputTestCase
            new QualityMeterTypeDescriptor(
                SequentialOutputTestCaseNetworkQualityMeter.TextName,
                (inner, parent, propagations, testCaseList) => SequentialOutputTestCaseNetworkQualityMeter.Parse(inner, parent, propagations, testCaseList)
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

        // Placeholder types for static parsing expansion
        _placeholderDescriptors = new List<PlaceholderTypeDescriptor>
        {
            // TestCasesSequential placeholder - expands to sequential test case meters with IfAllGood chain
            new PlaceholderTypeDescriptor(
                TestCasesSequentialContainerQualityMeter.TextName,
                ParseSequentialPlaceholder,
                ExpandSequentialPlaceholder
            ),
            // TestCases placeholder - expands to test case meters for each test case and propagation
            new PlaceholderTypeDescriptor(
                TestCasesContainerQualityMeter.TextName,
                ParseTestCasesPlaceholder,
                ExpandTestCasesPlaceholder
            )
        };

        _placeholderByTextName = _placeholderDescriptors.ToDictionary(d => d.TextName);
    }

    private static PlaceholderData? ParseSequentialPlaceholder(string inner)
    {
        int props = int.Parse(inner, CultureInfo.InvariantCulture);
        return new PlaceholderData(TestCasesSequentialContainerQualityMeter.TextName, props, props);
    }

    private static PlaceholderData? ParseTestCasesPlaceholder(string inner)
    {
        var (from, to) = CodedText.SplitRange(inner);
        return new PlaceholderData(TestCasesContainerQualityMeter.TextName, from, to);
    }

    private static void ExpandSequentialPlaceholder(
        PlaceholderData placeholder,
        QualityMeter<Network> parent,
        List<string> childSpecs,
        int propagations,
        TestCaseList testCaseList,
        Func<string, QualityMeter<Network>, int, TestCaseList, QualityMeter<Network>?> parseSingleMeter)
    {
        if (testCaseList == null) return;

        double goodDifference = 0.001d;
        QualityMeter<Network> currentParent = parent;

        foreach (TestCase testCase in testCaseList.TestCases)
        {
            var tcMeter = new TestCaseNetworkQualityMeter(currentParent, testCase, placeholder.PropagationsFrom);
            currentParent.Children.Add(tcMeter);

            // Add children to each test case meter
            foreach (string childSpec in childSpecs)
            {
                var childMeter = parseSingleMeter(childSpec, tcMeter, propagations, testCaseList);
                if (childMeter != null)
                {
                    tcMeter.Children.Add(childMeter);
                }
            }

            // Add IfAllGood and make it the parent for next test case
            var ifAllGoodMeter = new TestCasesIfAllGoodNetworkQualityMeter(currentParent, goodDifference);
            currentParent.Children.Add(ifAllGoodMeter);
            currentParent = ifAllGoodMeter;
        }
    }

    private static void ExpandTestCasesPlaceholder(
        PlaceholderData placeholder,
        QualityMeter<Network> parent,
        List<string> childSpecs,
        int propagations,
        TestCaseList testCaseList,
        Func<string, QualityMeter<Network>, int, TestCaseList, QualityMeter<Network>?> parseSingleMeter)
    {
        if (testCaseList == null) return;

        foreach (int props in Enumerable.Range(placeholder.PropagationsFrom, placeholder.PropagationsTo - placeholder.PropagationsFrom + 1))
        {
            foreach (TestCase testCase in testCaseList.TestCases)
            {
                var tcMeter = new TestCaseNetworkQualityMeter(parent, testCase, props);
                parent.Children.Add(tcMeter);

                // Add children to each test case meter
                foreach (string childSpec in childSpecs)
                {
                    var childMeter = parseSingleMeter(childSpec, tcMeter, propagations, testCaseList);
                    if (childMeter != null)
                    {
                        tcMeter.Children.Add(childMeter);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Try to parse a meter from its text name and parameters.
    /// </summary>
    public static QualityMeter<Network>? TryParse(
        string textName,
        string parameters,
        QualityMeter<Network> parent,
        int propagations,
        TestCaseList testCaseList)
    {
        if (_byTextName.TryGetValue(textName, out var descriptor))
        {
            return descriptor.Parser(parameters, parent, propagations, testCaseList);
        }
        return null;
    }

    /// <summary>
    /// Try to parse a container meter from its text name.
    /// </summary>
    public static QualityMeter<Network>? TryParseContainer(
        string textName,
        CodedLines lines,
        TestCaseList testCaseList,
        Func<string, QualityMeter<Network>, int, TestCaseList, QualityMeter<Network>?> singleMeterParser)
    {
        if (_containerByTextName.TryGetValue(textName, out var descriptor))
        {
            return descriptor.ContainerParser(lines, testCaseList, singleMeterParser);
        }
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
            {
                return descriptor;
            }
        }
        return null;
    }

    /// <summary>
    /// Try to parse a placeholder from its text name and parameters.
    /// Returns null if not a placeholder type.
    /// </summary>
    public static PlaceholderData? TryParsePlaceholder(string textName, string parameters)
    {
        if (_placeholderByTextName.TryGetValue(textName, out var descriptor))
        {
            return descriptor.ParsePlaceholder(parameters);
        }
        return null;
    }

    /// <summary>
    /// Checks if a text name is a placeholder type.
    /// </summary>
    public static bool IsPlaceholderType(string textName) => _placeholderByTextName.ContainsKey(textName);

    /// <summary>
    /// Expands a placeholder into actual meters.
    /// </summary>
    public static void ExpandPlaceholder(
        PlaceholderData placeholder,
        QualityMeter<Network> parent,
        List<string> childSpecs,
        int propagations,
        TestCaseList testCaseList,
        Func<string, QualityMeter<Network>, int, TestCaseList, QualityMeter<Network>?> parseSingleMeter)
    {
        if (_placeholderByTextName.TryGetValue(placeholder.TextName, out var descriptor))
        {
            descriptor.ExpandPlaceholder(placeholder, parent, childSpecs, propagations, testCaseList, parseSingleMeter);
        }
    }
}
