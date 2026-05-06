using System.Globalization;
using System.Text;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring.DiskStoring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;

/// <summary>
/// A quality meter container that creates sequential test case meters on the fly.
/// Each test case is followed by an IfAllGood meter, creating a chain where the IfAllGood
/// gates evaluation of subsequent test cases.
/// Structure: Container -> [TestCase1, IfAllGood1 -> [TestCase2, IfAllGood2 -> [TestCase3, ...]]]
/// </summary>
public class TestCasesSequentialContainerQualityMeter : QualityMeter<Network>, INetworkQualityMeterWithTestCaseList, INetworkQualityMeterWithPropagations, INetworkQualityMeterContainerTextConvertible
{
    private readonly double _goodDifference;
    private readonly Func<QualityMeter<Network>, TestCase, int, TestCaseNetworkQualityMeter> _testCaseMeterFactory;
    private readonly List<QualityMeter<Network>> _staticChildren = new();

    public TestCaseList? TestCaseList { get; set; }
    public int? Propagations { get; set; }

    public TestCasesSequentialContainerQualityMeter(
        double goodDifference,
        Func<QualityMeter<Network>, TestCase, int, TestCaseNetworkQualityMeter> testCaseMeterFactory)
        : base(null)
    {
        _goodDifference = goodDifference;
        _testCaseMeterFactory = testCaseMeterFactory;
    }

    public override List<QualityMeter<Network>>? Children
    {
        get
        {
            if (TestCaseList == null || Propagations == null)
                return null;

            List<QualityMeter<Network>> children = new List<QualityMeter<Network>>();

            if (TestCaseList.TestCases.Any())
            {
                List<TestCase> testCases = TestCaseList.TestCases.ToList();
                BuildSequentialChain(children, this, testCases, 0);
            }

            // Add static children (IfAllGood, etc.)
            foreach (QualityMeter<Network> staticChild in _staticChildren)
                children.Add(staticChild);

            return children;
        }
    }

    /// <summary>
    /// Adds a static child that persists across test case list changes.
    /// </summary>
    public void AddStaticChild(QualityMeter<Network> child)
    {
        _staticChildren.Add(child);
    }

    /// <summary>
    /// Gets the static children (non-test-case meters).
    /// </summary>
    public IEnumerable<QualityMeter<Network>> GetStaticChildren() => _staticChildren;

    private void BuildSequentialChain(List<QualityMeter<Network>> targetChildren, QualityMeter<Network> parent, List<TestCase> testCases, int index)
    {
        if (index >= testCases.Count)
            return;

        // Add test case meter for current index
        TestCaseNetworkQualityMeter testCaseMeter = _testCaseMeterFactory(parent, testCases[index], Propagations!.Value);
        targetChildren.Add(testCaseMeter);

        // Add IfAllGood meter that will contain the rest of the chain
        SequentialIfAllGoodNetworkQualityMeter ifAllGoodMeter = new SequentialIfAllGoodNetworkQualityMeter(
            parent,
            _goodDifference,
            testCases,
            index + 1,
            Propagations!.Value,
            _testCaseMeterFactory);
        targetChildren.Add(ifAllGoodMeter);
    }

    public void WriteToText(StringBuilder sb)
    {
        sb.AppendLine($"TestCasesSequential()");

        // Write PerTestCase section with children template from first test case meter
        if (TestCaseList != null && Propagations != null)
        {
            List<TestCaseNetworkQualityMeter> testCaseMeters = Children.Where(c => c is TestCaseNetworkQualityMeter).Cast<TestCaseNetworkQualityMeter>().ToList();
            if (testCaseMeters.Any())
            {
                sb.AppendLine($"  {NetworkQualityMeterSectionExtensions.PerTestCaseSection}");
                TestCaseNetworkQualityMeter templateMeter = testCaseMeters.First();
                foreach (QualityMeter<Network> child in templateMeter.Children)
                {
                    if (child is INetworkQualityMeterTextConvertible convertible)
                        sb.AppendLine($"    {convertible.ToText()}");
                }
            }
        }

        // Write AfterAllTestCases section with static children
        if (_staticChildren.Any())
        {
            sb.AppendLine($"  {AfterAllTestCasesSection}");
            foreach (QualityMeter<Network> staticChild in _staticChildren)
                WriteStaticChild(sb, staticChild, 2);
        }
    }

    private static void WriteStaticChild(StringBuilder sb, QualityMeter<Network> meter, int indentLevel)
    {
        string indent = new string(' ', indentLevel * 2);
        string? sectionHeader = meter.GetSectionHeader();

        if (sectionHeader != null)
            sb.AppendLine($"{indent}{sectionHeader}");

        // Write the meter itself
        if (meter is INetworkQualityMeterTextConvertible convertible)
            sb.AppendLine($"{indent}  {convertible.ToText()}");

        // Group children by primary section header (matches GetSectionHeader priority order:
        // an IfAllGood child counted as FromTestCases, never doubled in FromNetwork).
        List<QualityMeter<Network>> fromTestCasesChildren = meter.Children
            .Where(c => c.GetSectionHeader() == NetworkQualityMeterSectionExtensions.FromTestCasesSection)
            .ToList();
        List<QualityMeter<Network>> fromNetworkChildren = meter.Children
            .Where(c => c.GetSectionHeader() == NetworkQualityMeterSectionExtensions.FromNetworkSection)
            .ToList();

        if (fromTestCasesChildren.Any())
        {
            sb.AppendLine($"{indent}    {NetworkQualityMeterSectionExtensions.FromTestCasesSection}");
            foreach (QualityMeter<Network> child in fromTestCasesChildren)
            {
                if (child is INetworkQualityMeterTextConvertible childConvertible)
                    sb.AppendLine($"{indent}      {childConvertible.ToText()}");
            }
        }

        if (fromNetworkChildren.Any())
        {
            sb.AppendLine($"{indent}    {NetworkQualityMeterSectionExtensions.FromNetworkSection}");
            foreach (QualityMeter<Network> child in fromNetworkChildren)
            {
                if (child is INetworkQualityMeterTextConvertible childConvertible)
                    sb.AppendLine($"{indent}      {childConvertible.ToText()}");
            }
        }
    }

    public const string TextName = "TestCasesSequential";
    private const string AfterAllTestCasesSection = "AfterAllTestCases:";

    public static TestCasesSequentialContainerQualityMeter Parse(CodedText text, Func<string, QualityMeter<Network>, QualityMeter<Network>?> singleMeterParser)
    {
        int baseIndent = text.CurrentIndent;
        text.TrySkip(TextName);
        text.ReadParenthesesContent(); // ignore content
        text.AdvanceLine();

        // Collect child meter specs from PerTestCase section - REQUIRED
        if (text.EOT || text.CurrentLineContent != NetworkQualityMeterSectionExtensions.PerTestCaseSection)
            throw new InvalidOperationException($"Expected '{NetworkQualityMeterSectionExtensions.PerTestCaseSection}' section after TestCasesSequential(). Format requires explicit sections.");

        text.AdvanceLine();
        List<string> childSpecs = CollectSectionContent(text, baseIndent + 1);

        // Validate PerTestCase meters by parsing a probe and checking the interface.
        foreach (string spec in childSpecs)
        {
            QualityMeter<Network>? probe = singleMeterParser(spec, new QualityMeter<Network>(null));
            if (probe == null)
                throw new InvalidOperationException($"Unknown meter type in spec: {spec}");
            probe.ValidateInSection(NetworkQualityMeterSectionExtensions.PerTestCaseSection);
        }

        // Default goodDifference
        double goodDifference = 0.001d;

        // Build factory from child specs
        Func<QualityMeter<Network>, TestCase, int, TestCaseNetworkQualityMeter> factory = (parent, testCase, props) =>
        {
            TestCaseNetworkQualityMeter tcMeter = new TestCaseNetworkQualityMeter(parent, testCase, props);
            foreach (string spec in childSpecs)
            {
                QualityMeter<Network>? childMeter = singleMeterParser(spec, tcMeter);
                if (childMeter != null)
                    tcMeter.Children.Add(childMeter);
            }
            return tcMeter;
        };

        TestCasesSequentialContainerQualityMeter container = new TestCasesSequentialContainerQualityMeter(goodDifference, factory);

        // Parse AfterAllTestCases section - OPTIONAL but if present must use correct format
        if (!text.EOT && text.CurrentLineContent == AfterAllTestCasesSection)
        {
            text.AdvanceLine();
            ParseAfterAllTestCasesSection(text, container, singleMeterParser, baseIndent + 1);
        }
        else if (!text.EOT && !string.IsNullOrWhiteSpace(text.CurrentLineContent))
        {
            // There's content but it's not AfterAllTestCases: section
            throw new InvalidOperationException($"Expected '{AfterAllTestCasesSection}' section or end of file. Found: '{text.CurrentLineContent}'. Format requires explicit sections.");
        }

        return container;
    }

    private static List<string> CollectSectionContent(CodedText text, int sectionIndent)
    {
        List<string> specs = new();
        while (!text.EOT)
        {
            int currentIndent = text.CurrentIndent;
            if (currentIndent <= sectionIndent)
                break;

            string? content = text.CurrentLineContent;
            if (!string.IsNullOrWhiteSpace(content) && !content.EndsWith(":"))
                specs.Add(content);

            text.AdvanceLine();
        }
        return specs;
    }

    private static void ParseAfterAllTestCasesSection(
        CodedText text,
        TestCasesSequentialContainerQualityMeter container,
        Func<string, QualityMeter<Network>, QualityMeter<Network>?> singleMeterParser,
        int sectionIndent)
    {
        while (!text.EOT)
        {
            int currentIndent = text.CurrentIndent;
            if (currentIndent <= sectionIndent)
                break;

            string? content = text.CurrentLineContent;
            if (string.IsNullOrWhiteSpace(content))
            {
                text.AdvanceLine();
                continue;
            }

            if (content == NetworkQualityMeterSectionExtensions.FromTestCasesSection || content == NetworkQualityMeterSectionExtensions.FromNetworkSection)
            {
                string sectionHeader = content;
                text.AdvanceLine();
                ParseDataSourceSection(text, container, singleMeterParser, currentIndent, sectionHeader);
            }
            else
            {
                throw new InvalidOperationException($"Expected '{NetworkQualityMeterSectionExtensions.FromTestCasesSection}' or '{NetworkQualityMeterSectionExtensions.FromNetworkSection}' section inside AfterAllTestCases. Found: '{content}'. Format requires explicit data source sections.");
            }
        }
    }

    private static void ParseDataSourceSection(
        CodedText text,
        TestCasesSequentialContainerQualityMeter container,
        Func<string, QualityMeter<Network>, QualityMeter<Network>?> singleMeterParser,
        int sectionIndent,
        string expectedSectionHeader)
    {
        while (!text.EOT)
        {
            int currentIndent = text.CurrentIndent;
            if (currentIndent <= sectionIndent)
                break;

            string? content = text.CurrentLineContent;
            if (string.IsNullOrWhiteSpace(content))
            {
                text.AdvanceLine();
                continue;
            }

            if (content == NetworkQualityMeterSectionExtensions.FromTestCasesSection || content == NetworkQualityMeterSectionExtensions.FromNetworkSection)
                break;

            QualityMeter<Network>? meter = singleMeterParser(content, container);
            if (meter != null)
            {
                meter.ValidateInSection(expectedSectionHeader);
                container.AddStaticChild(meter);
                text.AdvanceLine();

                // Parse children of static meter (nested sections)
                ParseStaticChildChildren(text, meter, singleMeterParser, currentIndent);
            }
            else
            {
                text.AdvanceLine();
            }
        }
    }

    private static void ParseStaticChildChildren(
        CodedText text,
        QualityMeter<Network> parent,
        Func<string, QualityMeter<Network>, QualityMeter<Network>?> singleMeterParser,
        int parentIndent)
    {
        while (!text.EOT)
        {
            int currentIndent = text.CurrentIndent;
            if (currentIndent <= parentIndent)
                break;

            string? content = text.CurrentLineContent;
            if (string.IsNullOrWhiteSpace(content))
            {
                text.AdvanceLine();
                continue;
            }

            if (content == NetworkQualityMeterSectionExtensions.FromTestCasesSection || content == NetworkQualityMeterSectionExtensions.FromNetworkSection)
            {
                string sectionHeader = content;
                text.AdvanceLine();
                ParseChildMetersInSection(text, parent, singleMeterParser, currentIndent, sectionHeader);
            }
            else
            {
                throw new InvalidOperationException($"Expected '{NetworkQualityMeterSectionExtensions.FromTestCasesSection}' or '{NetworkQualityMeterSectionExtensions.FromNetworkSection}' section for child meters. Found: '{content}'. Format requires explicit data source sections.");
            }
        }
    }

    private static void ParseChildMetersInSection(
        CodedText text,
        QualityMeter<Network> parent,
        Func<string, QualityMeter<Network>, QualityMeter<Network>?> singleMeterParser,
        int sectionIndent,
        string expectedSectionHeader)
    {
        while (!text.EOT)
        {
            int currentIndent = text.CurrentIndent;
            if (currentIndent <= sectionIndent)
                break;

            string? content = text.CurrentLineContent;
            if (string.IsNullOrWhiteSpace(content))
            {
                text.AdvanceLine();
                continue;
            }

            if (content == NetworkQualityMeterSectionExtensions.FromTestCasesSection || content == NetworkQualityMeterSectionExtensions.FromNetworkSection)
                break;

            QualityMeter<Network>? childMeter = singleMeterParser(content, parent);
            if (childMeter != null)
            {
                childMeter.ValidateInSection(expectedSectionHeader);
                parent.Children.Add(childMeter);
            }

            text.AdvanceLine();
        }
    }
}

/// <summary>
/// A specialized IfAllGood meter that dynamically generates its children for sequential test case evaluation.
/// </summary>
internal class SequentialIfAllGoodNetworkQualityMeter : TestCasesIfAllGoodNetworkQualityMeter
{
    private readonly List<TestCase> _testCases;
    private readonly int _startIndex;
    private readonly int _propagations;
    private readonly Func<QualityMeter<Network>, TestCase, int, TestCaseNetworkQualityMeter> _testCaseMeterFactory;

    public SequentialIfAllGoodNetworkQualityMeter(
        QualityMeter<Network> parent,
        double goodDifference,
        List<TestCase> testCases,
        int startIndex,
        int propagations,
        Func<QualityMeter<Network>, TestCase, int, TestCaseNetworkQualityMeter> testCaseMeterFactory)
        : base(parent, goodDifference)
    {
        _testCases = testCases;
        _startIndex = startIndex;
        _propagations = propagations;
        _testCaseMeterFactory = testCaseMeterFactory;
    }

    public override List<QualityMeter<Network>> Children
    {
        get
        {
            var children = new List<QualityMeter<Network>>();

            if (_startIndex < _testCases.Count)
            {
                // Add test case meter for current index
                var testCaseMeter = _testCaseMeterFactory(this, _testCases[_startIndex], _propagations);
                children.Add(testCaseMeter);

                // Add next IfAllGood in the chain if there are more test cases
                if (_startIndex + 1 < _testCases.Count)
                {
                    var nextIfAllGoodMeter = new SequentialIfAllGoodNetworkQualityMeter(
                        this,
                        GoodDifference,
                        _testCases,
                        _startIndex + 1,
                        _propagations,
                        _testCaseMeterFactory);
                    children.Add(nextIfAllGoodMeter);
                }
            }

            return children;
        }
    }
}
