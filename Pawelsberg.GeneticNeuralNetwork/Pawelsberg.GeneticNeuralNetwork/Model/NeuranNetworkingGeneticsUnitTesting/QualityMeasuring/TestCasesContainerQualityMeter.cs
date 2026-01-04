using System.Globalization;
using System.Text;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting.QualityMeasuring.DiskStoring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting.QualityMeasuring;

/// <summary>
/// A quality meter container that creates child meters for each test case on the fly.
/// Children are computed each time they are accessed based on current TestCaseList and Propagations.
/// </summary>
public class TestCasesContainerQualityMeter : QualityMeter<Network>, ITestCasesQualityMeterContainer, INetworkQualityMeterContainerTextConvertible
{
    private TestCaseList _testCaseList;
    private int _propagations;
    private int? _propagationsTo; // For propagation range support
    private readonly Func<QualityMeter<Network>, TestCase, int, TestCaseNetworkQualityMeter> _testCaseMeterFactory;
    private readonly List<QualityMeter<Network>> _staticChildren = new();

    public TestCaseList TestCaseList
    {
        get => _testCaseList;
        set => _testCaseList = value;
    }

    public int Propagations
    {
        get => _propagations;
        set => _propagations = value;
    }

    /// <summary>
    /// Constructor for single propagation value.
    /// </summary>
    public TestCasesContainerQualityMeter(
        TestCaseList testCaseList,
        int propagations,
        Func<QualityMeter<Network>, TestCase, int, TestCaseNetworkQualityMeter> testCaseMeterFactory)
        : base(null)
    {
        _testCaseList = testCaseList;
        _propagations = propagations;
        _propagationsTo = null;
        _testCaseMeterFactory = testCaseMeterFactory;
    }

    /// <summary>
    /// Constructor for propagation range (PropagationsAgnostic mode).
    /// </summary>
    public TestCasesContainerQualityMeter(
        TestCaseList testCaseList,
        int propagationsFrom,
        int propagationsTo,
        Func<QualityMeter<Network>, TestCase, int, TestCaseNetworkQualityMeter> testCaseMeterFactory)
        : base(null)
    {
        _testCaseList = testCaseList;
        _propagations = propagationsFrom;
        _propagationsTo = propagationsTo;
        _testCaseMeterFactory = testCaseMeterFactory;
    }

    public override List<QualityMeter<Network>> Children
    {
        get
        {
            var children = new List<QualityMeter<Network>>();
            
            // Add dynamically created test case meters
            if (_testCaseList != null)
            {
                int from = _propagations;
                int to = _propagationsTo ?? _propagations;
                
                foreach (int props in Enumerable.Range(from, to - from + 1))
                {
                    foreach (TestCase testCase in _testCaseList.TestCases)
                    {
                        children.Add(_testCaseMeterFactory(this, testCase, props));
                    }
                }
            }
            
            // Add static children (IfAllGood, etc.)
            foreach (var staticChild in _staticChildren)
            {
                children.Add(staticChild);
            }
            
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
    public (int From, int To) GetPropagationRange()
    {
        return (_propagations, _propagationsTo ?? _propagations);
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
        var testCaseMeters = Children.Where(c => c is TestCaseNetworkQualityMeter).Cast<TestCaseNetworkQualityMeter>().ToList();
        
        if (testCaseMeters.Any())
        {
            var distinctProps = testCaseMeters.Select(m => m.Propagations).Distinct().OrderBy(p => p).ToList();
            int propsFrom = distinctProps.First();
            int propsTo = distinctProps.Last();
            
            if (propsFrom == propsTo)
            {
                sb.AppendLine($"TestCases({propsFrom.ToString(CultureInfo.InvariantCulture)})");
            }
            else
            {
                sb.AppendLine($"TestCases({propsFrom.ToString(CultureInfo.InvariantCulture)}-{propsTo.ToString(CultureInfo.InvariantCulture)})");
            }
            
            // Write children template from first test case meter
            var templateMeter = testCaseMeters.First();
            foreach (var child in templateMeter.Children)
            {
                if (child is INetworkQualityMeterTextConvertible convertible)
                {
                    sb.AppendLine($"  {convertible.ToText()}");
                }
            }
        }
        
        // Write static children
        foreach (var child in _staticChildren)
        {
            WriteStaticChild(sb, child, 0);
        }
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

    public static TestCasesContainerQualityMeter Parse(CodedLines lines, TestCaseList testCaseList, Func<string, QualityMeter<Network>, int, TestCaseList, QualityMeter<Network>?> singleMeterParser)
    {
        string firstLine = lines[0].Trim();
        CodedText codedText = new CodedText(firstLine);
        codedText.TrySkip(TextName);
        string inner = codedText.ReadParenthesesContent();
        
        var (propsFrom, propsTo) = CodedText.SplitRange(inner);

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
                var childMeter = singleMeterParser(spec, tcMeter, props, null);
                if (childMeter != null)
                {
                    tcMeter.Children.Add(childMeter);
                }
            }
            return tcMeter;
        };

        // Create container
        TestCasesContainerQualityMeter container;
        if (propsFrom == propsTo)
        {
            container = new TestCasesContainerQualityMeter(testCaseList, propsFrom, factory);
        }
        else
        {
            container = new TestCasesContainerQualityMeter(testCaseList, propsFrom, propsTo, factory);
        }

        // Parse static children (meters at same indent as TestCases, after child specs)
        while (!lines.EndOfLines)
        {
            int indent = lines.CurrentIndent;
            string? content = lines.CurrentContent;
            
            if (string.IsNullOrEmpty(content))
            {
                lines.Advance();
                continue;
            }

            var staticMeter = singleMeterParser(content, container, propsFrom, null);
            if (staticMeter != null)
            {
                container.AddStaticChild(staticMeter);
                lines.Advance();
                
                // Parse children of static meter
                int staticMeterIndent = indent;
                while (!lines.EndOfLines)
                {
                    int childIndent = lines.CurrentIndent;
                    if (childIndent <= staticMeterIndent)
                        break;
                    
                    var childMeter = singleMeterParser(lines.CurrentContent!, staticMeter, propsFrom, null);
                    if (childMeter != null)
                    {
                        staticMeter.Children.Add(childMeter);
                    }
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
