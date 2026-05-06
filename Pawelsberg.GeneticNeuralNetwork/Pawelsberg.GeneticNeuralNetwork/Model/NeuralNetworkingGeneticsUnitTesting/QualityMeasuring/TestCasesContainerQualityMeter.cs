using System.Globalization;
using System.Text;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring.DiskStoring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;

/// <summary>
/// A quality meter container that creates child meters for each test case on the fly.
/// Children are computed each time they are accessed based on current TestCaseList and Propagations.
/// </summary>
public class TestCasesContainerQualityMeter : QualityMeter<Network>, INetworkQualityMeterWithTestCaseList, INetworkQualityMeterWithPropagations, INetworkQualityMeterContainerTextConvertible
{
    private int _additionalPropagations;
    private readonly Func<QualityMeter<Network>, TestCase, int, TestCaseNetworkQualityMeter> _testCaseMeterFactory;
    private readonly List<QualityMeter<Network>> _staticChildren = new();

    public TestCaseList? TestCaseList { get; set; }
    public int? Propagations { get; set; }

    public int AdditionalPropagations
    {
        get => _additionalPropagations;
        set => _additionalPropagations = value;
    }

    public TestCasesContainerQualityMeter(
        Func<QualityMeter<Network>, TestCase, int, TestCaseNetworkQualityMeter> testCaseMeterFactory,
        int additionalPropagations = 0)
        : base(null)
    {
        _testCaseMeterFactory = testCaseMeterFactory;
        _additionalPropagations = additionalPropagations;
    }

    public override List<QualityMeter<Network>>? Children
    {
        get
        {
            if (TestCaseList == null || Propagations == null)
                return null;

            List<QualityMeter<Network>> children = new List<QualityMeter<Network>>();

            int from = Propagations.Value;
            int to = Propagations.Value + _additionalPropagations;

            foreach (int props in Enumerable.Range(from, to - from + 1))
            {
                foreach (TestCase testCase in TestCaseList.TestCases)
                    children.Add(_testCaseMeterFactory(this, testCase, props));
            }

            // Add static children (IfAllGood, etc.)
            foreach (var staticChild in _staticChildren)
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
    /// Gets a template test case meter for serialization.
    /// </summary>
    public TestCaseNetworkQualityMeter? GetTemplateMeter()
    {
        var testCaseMeters = Children.Where(c => c is TestCaseNetworkQualityMeter).Cast<TestCaseNetworkQualityMeter>().ToList();
        return testCaseMeters.FirstOrDefault();
    }

    public void WriteToText(StringBuilder sb)
    {
        if (_additionalPropagations == 0)
            sb.AppendLine($"TestCases()");
        else
            sb.AppendLine($"TestCases({_additionalPropagations.ToString(CultureInfo.InvariantCulture)})");

        // Write PerTestCase section with children template from first test case meter
        TestCaseNetworkQualityMeter? templateMeter = GetTemplateMeter();
        if (templateMeter != null && templateMeter.Children.Any())
        {
            sb.AppendLine($"  {NetworkQualityMeterSectionExtensions.PerTestCaseSection}");
            foreach (QualityMeter<Network> child in templateMeter.Children)
            {
                if (child is INetworkQualityMeterTextConvertible convertible)
                    sb.AppendLine($"    {convertible.ToText()}");
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

    public const string TextName = "TestCases";
    private const string AfterAllTestCasesSection = "AfterAllTestCases:";

    public static TestCasesContainerQualityMeter Parse(CodedText text, Func<string, QualityMeter<Network>, QualityMeter<Network>?> singleMeterParser)
    {
        int baseIndent = text.CurrentIndent;
        text.TrySkip(TextName);
        string inner = text.ReadParenthesesContent();
        int additionalPropagations = string.IsNullOrWhiteSpace(inner) ? 0 : int.Parse(inner.Trim(), CultureInfo.InvariantCulture);
        text.AdvanceLine();

        // Collect child meter specs from PerTestCase section - REQUIRED
        if (text.EOT || text.CurrentLineContent != NetworkQualityMeterSectionExtensions.PerTestCaseSection)
            throw new InvalidOperationException($"Expected '{NetworkQualityMeterSectionExtensions.PerTestCaseSection}' section after TestCases(). Format requires explicit sections.");

        text.AdvanceLine();
        int sectionIndent = baseIndent + 1;
        List<string> perTestCaseSpecs = CollectSectionContent(text, sectionIndent);

        // Validate PerTestCase meters by parsing a probe and checking the interface.
        foreach (string spec in perTestCaseSpecs)
        {
            QualityMeter<Network>? probe = singleMeterParser(spec, new QualityMeter<Network>(null));
            if (probe == null)
                throw new InvalidOperationException($"Unknown meter type in spec: {spec}");
            probe.ValidateInSection(NetworkQualityMeterSectionExtensions.PerTestCaseSection);
        }

        // Build factory from child specs
        Func<QualityMeter<Network>, TestCase, int, TestCaseNetworkQualityMeter> factory = (parent, testCase, props) =>
        {
            TestCaseNetworkQualityMeter tcMeter = new TestCaseNetworkQualityMeter(parent, testCase, props);
            foreach (string spec in perTestCaseSpecs)
            {
                QualityMeter<Network>? childMeter = singleMeterParser(spec, tcMeter);
                if (childMeter != null)
                    tcMeter.Children.Add(childMeter);
            }
            return tcMeter;
        };

        TestCasesContainerQualityMeter container = new TestCasesContainerQualityMeter(factory, additionalPropagations);

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
        TestCasesContainerQualityMeter container,
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
        TestCasesContainerQualityMeter container,
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
