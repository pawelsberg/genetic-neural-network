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
            
            return children;
        }
    }

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
        
        // Write children template from first test case meter - need to check if configured
        if (TestCaseList != null && Propagations != null)
        {
            List<TestCaseNetworkQualityMeter> testCaseMeters = Children.Where(c => c is TestCaseNetworkQualityMeter).Cast<TestCaseNetworkQualityMeter>().ToList();
            if (testCaseMeters.Any())
            {
                TestCaseNetworkQualityMeter templateMeter = testCaseMeters.First();
                foreach (QualityMeter<Network> child in templateMeter.Children)
                {
                    if (child is INetworkQualityMeterTextConvertible convertible)
                        sb.AppendLine($"  {convertible.ToText()}");
                }
            }
        }
    }

    public const string TextName = "TestCasesSequential";

    public static TestCasesSequentialContainerQualityMeter Parse(CodedLines lines, Func<string, QualityMeter<Network>, QualityMeter<Network>?> singleMeterParser)
    {
        string firstLine = lines[0].Trim();
        CodedText codedText = new CodedText(firstLine);
        codedText.TrySkip(TextName);
        codedText.ReadParenthesesContent(); // ignore content
        
        // Collect child meter specs
        int baseIndent = CodedText.GetIndentLevel(lines[0]);
        lines.Index = 1;
        var childSpecs = lines.CollectIndentedContent(baseIndent);

        // Default goodDifference
        double goodDifference = 0.001d;

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

        return new TestCasesSequentialContainerQualityMeter(goodDifference, factory);
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
