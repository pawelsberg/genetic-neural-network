using System.Globalization;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;

public class SequentialOutputTestCaseNetworkQualityMeter : QualityMeter<Network>, INetworkQualityMeterTextConvertible, INetworkQualityMeterWithPropagations, INetworkQualityMeterWithTestCaseList
{
    public static string TextName = "SequentialOutputTestCase";

    public double MaxDifferencePerTestOutput { get; }
    public double MaxQualityPerTestOutput { get; }
    public double MaxQualityForExistingInputsOutputs { get; }
    public TestCaseList TestCaseList { get; set; }
    public int Propagations { get; set; }

    public SequentialOutputTestCaseNetworkQualityMeter(
        QualityMeter<Network> parent,
        double maxDifferencePerTestOutput,
        double maxQualityPerTestOutput,
        double maxQualityForExistingInputsOutputs) : base(parent)
    {
        MaxDifferencePerTestOutput = maxDifferencePerTestOutput;
        MaxQualityPerTestOutput = maxQualityPerTestOutput;
        MaxQualityForExistingInputsOutputs = maxQualityForExistingInputsOutputs;
    }

    public string ToText() => $"{TextName}({MaxDifferencePerTestOutput.ToString(CultureInfo.InvariantCulture)},{MaxQualityPerTestOutput.ToString(CultureInfo.InvariantCulture)},{MaxQualityForExistingInputsOutputs.ToString(CultureInfo.InvariantCulture)})";

    public override QualityMeasurement<Network> MeasureMeterQuality(Network network, QualityMeasurement<Network> parentQualityMeasurement)
    {
        if (TestCaseList == null || TestCaseList.TestCases.Count == 0)
            return new QualityMeasurement<Network>(this, parentQualityMeasurement) { Quality = 0 };

        double totalQuality = 0d;

        int testCaseListOutputCount = TestCaseList.TestCases.Max(tc => tc.Outputs.Count);
        int networkOutputCount = network.Outputs.Count;
        int testCaseListInputCount = TestCaseList.TestCases.Max(tc => tc.Inputs.Count);
        int networkInputCount = network.Inputs.Count;

        if (testCaseListOutputCount > networkOutputCount || testCaseListInputCount > networkInputCount)
        {
            return new QualityMeasurement<Network>(this, parentQualityMeasurement)
            {
                Quality = MaxQualityForExistingInputsOutputs
                / (testCaseListOutputCount + testCaseListInputCount)
                * (Math.Min(networkOutputCount, testCaseListOutputCount) + Math.Min(networkInputCount, testCaseListInputCount))
            };
        }

        totalQuality += MaxQualityForExistingInputsOutputs;

        List<(TestCase TestCase, RunningContext RunningContext)> testCaseRunningContexts = 
            TestCaseList
            .TestCases
            .Select(tc => (tc, network.SafeRun(tc, Propagations)))
            .ToList();

        foreach (int outputIndex in Enumerable.Range(0, testCaseListOutputCount))
        {
            double outputIndexDifference = 0d;
            List<(TestCase TestCase, RunningContext RunningContext)> testCaseRunningContextsWithOutputIndex = 
                testCaseRunningContexts.Where(trc => trc.TestCase.Outputs.Count > outputIndex).ToList();
            
            int outputIndexTestCaseCount = testCaseRunningContextsWithOutputIndex.Count;

            foreach ((TestCase testCase, RunningContext runningContext) in testCaseRunningContextsWithOutputIndex)
            {
                double expectedOutput = testCase.Outputs[outputIndex];
                double actualOutput = runningContext.GetPotential(network.Outputs[outputIndex]);
                double outputDifference = Math.Abs(expectedOutput - actualOutput);
                outputIndexDifference += outputDifference;
            }

            if (outputIndexDifference > MaxDifferencePerTestOutput * outputIndexTestCaseCount)
            {
                return new QualityMeasurement<Network>(this, parentQualityMeasurement) { Quality = totalQuality + MaxQualityPerTestOutput / (outputIndexDifference + 1) };
            }

            totalQuality += MaxQualityPerTestOutput;
        }
        return new QualityMeasurement<Network>(this, parentQualityMeasurement) { Quality = totalQuality };
    }
    protected override double MaxMeterQuality
    {
        get
        {
            if (TestCaseList == null || TestCaseList.TestCases.Count == 0)
                return 0;
            return MaxQualityForExistingInputsOutputs + TestCaseList.TestCases.Max(tc => tc.Outputs.Count) * MaxQualityPerTestOutput;
        }
    }
}



