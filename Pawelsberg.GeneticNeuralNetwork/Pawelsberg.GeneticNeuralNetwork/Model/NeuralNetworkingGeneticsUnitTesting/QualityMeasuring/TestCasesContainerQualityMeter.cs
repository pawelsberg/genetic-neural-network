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
    /// Gets the propagation range for this container.
    /// </summary>
    public (int From, int To)? GetPropagationRange()
    {
        if (Propagations == null)
            return null;
        return (Propagations.Value, Propagations.Value + _additionalPropagations);
    }

    /// <summary>
    /// Gets the static children (non-test-case meters).
    /// </summary>
    public IEnumerable<QualityMeter<Network>> GetStaticChildren() => _staticChildren;

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
        
        // Write children template from first test case meter
        var templateMeter = GetTemplateMeter();
        if (templateMeter != null)
        {
            foreach (var child in templateMeter.Children)
            {
                if (child is INetworkQualityMeterTextConvertible convertible)
                    sb.AppendLine($"  {convertible.ToText()}");
            }
        }
        
        // Write static children
        foreach (var child in _staticChildren)
            WriteStaticChild(sb, child, 0);
    }

    private static void WriteStaticChild(StringBuilder sb, QualityMeter<Network> meter, int indentLevel)
    {
        string indent = new string(' ', indentLevel * 2);
        
        if (meter is INetworkQualityMeterTextConvertible convertible)
        {
            sb.AppendLine($"{indent}{convertible.ToText()}");
        }
        
        foreach (var child in meter.Children)
        {
            WriteStaticChild(sb, child, indentLevel + 1);
        }
    }

    public const string TextName = "TestCases";

    public static TestCasesContainerQualityMeter Parse(CodedLines lines, Func<string, QualityMeter<Network>, QualityMeter<Network>?> singleMeterParser)
    {
        string firstLine = lines[0].Trim();
        CodedText codedText = new CodedText(firstLine);
        codedText.TrySkip(TextName);
        string inner = codedText.ReadParenthesesContent();
        
        int additionalPropagations = string.IsNullOrWhiteSpace(inner) ? 0 : int.Parse(inner.Trim(), CultureInfo.InvariantCulture);

        // Collect child meter specs (indented lines after TestCases)
        int baseIndent = CodedText.GetIndentLevel(lines[0]);
        lines.Index = 1;
        var childSpecs = lines.CollectIndentedContent(baseIndent);

        // Build factory from child specs
        Func<QualityMeter<Network>, TestCase, int, TestCaseNetworkQualityMeter> factory = (parent, testCase, props) =>
        {
            var tcMeter = new TestCaseNetworkQualityMeter(parent, testCase, props);
            foreach (string spec in childSpecs)
            {
                var childMeter = singleMeterParser(spec, tcMeter);
                if (childMeter != null)
                    tcMeter.Children.Add(childMeter);
            }
            return tcMeter;
        };

        TestCasesContainerQualityMeter container = new TestCasesContainerQualityMeter(factory, additionalPropagations);

        // Parse static children (meters at same indent as TestCases, after child specs)
        while (!lines.EndOfLines)
        {
            string? content = lines.CurrentContent;
            
            if (string.IsNullOrEmpty(content))
            {
                lines.Advance();
                continue;
            }

            var staticMeter = singleMeterParser(content, container);
            if (staticMeter != null)
            {
                container.AddStaticChild(staticMeter);
                lines.Advance();
                
                // Parse children of static meter
                int staticMeterIndent = lines.CurrentIndent;
                while (!lines.EndOfLines)
                {
                    int childIndent = lines.CurrentIndent;
                    if (childIndent <= staticMeterIndent)
                        break;
                    
                    var childMeter = singleMeterParser(lines.CurrentContent!, staticMeter);
                    if (childMeter != null)
                        staticMeter.Children.Add(childMeter);
                    lines.Advance();
                }
            }
            else
            {
                lines.Advance();
            }
        }

        return container;
    }
}
