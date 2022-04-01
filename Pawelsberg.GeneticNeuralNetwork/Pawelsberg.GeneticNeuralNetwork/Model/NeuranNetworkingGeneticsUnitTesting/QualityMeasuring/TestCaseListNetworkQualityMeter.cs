using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting.DiskStoring;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting.QualityMeasuring;

public class TestCaseListNetworkQualityMeter : QualityMeter<Network>
{
    public TestCaseList TestCaseList { get; set; }
    public int Propagations { get; set; }

    public TestCaseListNetworkQualityMeter(QualityMeter<Network> parent, TestCaseList testCaseList, int propagations)
        : base(parent)
    {
        TestCaseList = testCaseList;
        Propagations = propagations;
    }

    public override QualityMeasurement<Network> MeasureMeterQuality(Network network, QualityMeasurement<Network> parentQualityMeasurement)
    {
        QualityMeasurement<Network> result = new QualityMeasurement<Network>(this, parentQualityMeasurement);

        // simple validation - number of inputs and outputs
        // TODO - consider separating 
        if (network.Inputs.Count < TestCaseList.TestCases.Max(tc => tc.Inputs.Count) || network.Outputs.Count < TestCaseList.TestCases.Max(tc => tc.Outputs.Count))
        {
            result.Quality = network.Inputs.Count < TestCaseList.TestCases.Max(tc => tc.Inputs.Count) ? (double)network.Inputs.Count / (TestCaseList.TestCases.Max(tc => tc.Inputs.Count) + 1) : 1d;
            result.Quality += network.Outputs.Count < TestCaseList.TestCases.Max(tc => tc.Outputs.Count) ? (double)network.Outputs.Count / (TestCaseList.TestCases.Max(tc => tc.Outputs.Count) + 1) : 1d;
            return result;
        }

        double outputValuesDifference = 0d;

        foreach (TestCase testCase in TestCaseList.TestCases)
        {
            RunningContext runningContext = network.SafeRun(testCase, Propagations);
            for (int outputIndex = 0; outputIndex < testCase.Outputs.Count; outputIndex++)
                outputValuesDifference += Math.Abs(testCase.Outputs[outputIndex] - runningContext.GetPotential(network.Outputs[outputIndex]));
        }


        result.Quality = 20d + 100d / (outputValuesDifference + 1d);
        return result;
    }
    protected override double MaxMeterQuality
    {
        get
        {
            return 120d;
        }
    }
}



