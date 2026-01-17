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
    private TestCaseList _testCaseList;
    private int _propagations;
    private readonly double _goodDifference;
    private readonly Func<QualityMeter<Network>, TestCase, int, TestCaseNetworkQualityMeter> _testCaseMeterFactory;

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

    public TestCasesSequentialContainerQualityMeter(
        TestCaseList testCaseList,
        int propagations,
        double goodDifference,
        Func<QualityMeter<Network>, TestCase, int, TestCaseNetworkQualityMeter> testCaseMeterFactory)
        : base(null)
    {
        _testCaseList = testCaseList;
        _propagations = propagations;
        _goodDifference = goodDifference;
        _testCaseMeterFactory = testCaseMeterFactory;
    }

    public override List<QualityMeter<Network>> Children
    {
        get
        {
            var children = new List<QualityMeter<Network>>();
            
            if (_testCaseList != null && _testCaseList.TestCases.Any())
            {
                var testCases = _testCaseList.TestCases.ToList();
                
                // Build nested structure recursively
                // Container children: [TestCase1, IfAllGood1]
                // IfAllGood1 children: [TestCase2, IfAllGood2]
                // etc.
                BuildSequentialChain(children, this, testCases, 0);
            }
            
            return children;
        }
    }

    private void BuildSequentialChain(List<QualityMeter<Network>> targetChildren, QualityMeter<Network> parent, List<TestCase> testCases, int index)
    {
        if (index >= testCases.Count)
            return;

        // Add test case meter for current index
        var testCaseMeter = _testCaseMeterFactory(parent, testCases[index], _propagations);
        targetChildren.Add(testCaseMeter);

        // Add IfAllGood meter that will contain the rest of the chain
        var ifAllGoodMeter = new SequentialIfAllGoodNetworkQualityMeter(
            parent, 
            _goodDifference, 
            testCases, 
            index + 1, 
            _propagations, 
            _testCaseMeterFactory);
        targetChildren.Add(ifAllGoodMeter);
    }

    public void WriteToText(StringBuilder sb)
    {
        sb.AppendLine($"TestCasesSequential({_propagations.ToString(CultureInfo.InvariantCulture)})");
        
        // Write children template from first test case meter
        var testCaseMeters = Children.Where(c => c is TestCaseNetworkQualityMeter).Cast<TestCaseNetworkQualityMeter>().ToList();
        if (testCaseMeters.Any())
        {
            var templateMeter = testCaseMeters.First();
            foreach (var child in templateMeter.Children)
            {
                if (child is INetworkQualityMeterTextConvertible convertible)
                {
                    sb.AppendLine($"  {convertible.ToText()}");
                }
            }
        }
    }

    public const string TextName = "TestCasesSequential";

    public static TestCasesSequentialContainerQualityMeter Parse(CodedLines lines, TestCaseList testCaseList, Func<string, QualityMeter<Network>, int, TestCaseList, QualityMeter<Network>?> singleMeterParser)
    {
        string firstLine = lines[0].Trim();
        CodedText codedText = new CodedText(firstLine);
        codedText.TrySkip(TextName);
        string inner = codedText.ReadParenthesesContent();
        int props = int.Parse(inner, CultureInfo.InvariantCulture);
        
        // Collect child meter specs
        int baseIndent = CodedText.GetIndentLevel(lines[0]);
        lines.Index = 1;
        var childSpecs = lines.CollectIndentedContent(baseIndent);

        // Default goodDifference
        double goodDifference = 0.001d;

        // Build factory from child specs
        Func<QualityMeter<Network>, TestCase, int, TestCaseNetworkQualityMeter> factory = (parent, testCase, propagations) =>
        {
            var tcMeter = new TestCaseNetworkQualityMeter(parent, testCase, propagations);
            foreach (string spec in childSpecs)
            {
                var childMeter = singleMeterParser(spec, tcMeter, propagations, null);
                if (childMeter != null)
                {
                    tcMeter.Children.Add(childMeter);
                }
            }
            return tcMeter;
        };

        return new TestCasesSequentialContainerQualityMeter(testCaseList, props, goodDifference, factory);
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
