using System.Globalization;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting.DiskStoring;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;

public class TestCaseNetworkQualityMeter : QualityMeter<Network>, INetworkQualityMeterTextConvertible, INetworkQualityMeterWithPropagations
{
    public static string TextName = "TestCase";
    public TestCase TestCase { get; set; }
    public int Propagations { get; set; }

    public TestCaseNetworkQualityMeter(QualityMeter<Network> parent, TestCase testCase, int propagations) : base(parent)
    {
        TestCase = testCase;
        Propagations = propagations;
    }

    public string ToText() => $"{TextName}({Propagations.ToString(CultureInfo.InvariantCulture)})";

    public override QualityMeasurement<Network> MeasureMeterQuality(Network network, QualityMeasurement<Network> parentQualityMeasurement)
    {
        TestCaseQualityMeasurement result = new TestCaseQualityMeasurement(this, parentQualityMeasurement);

        // simple validation - number of inputs and outputs
        // TODO - consider separating 
        if (network.Inputs.Count < TestCase.Inputs.Count || network.Outputs.Count < TestCase.Outputs.Count)
        {
            result.Quality = network.Inputs.Count < TestCase.Inputs.Count ? (double)network.Inputs.Count / (TestCase.Inputs.Count + 1) : 1d;
            result.Quality += network.Outputs.Count < TestCase.Outputs.Count ? (double)network.Outputs.Count / (TestCase.Outputs.Count + 1) : 1d;
            result.RunningTime = TimeSpan.MaxValue;
            result.OutputValuesDifference = double.MaxValue;
            return result;
        }

        DateTime start = DateTime.Now;
        RunningContext runningContext = network.SafeRun(TestCase, Propagations);
        DateTime stop = DateTime.Now;
        result.RunningTime = stop.Subtract(start);

        double outputValuesDifference = 0d;
        for (int outputIndex = 0; outputIndex < TestCase.Outputs.Count; outputIndex++)
        {
            outputValuesDifference += Math.Abs(TestCase.Outputs[outputIndex] - runningContext.GetPotential(network.Outputs[outputIndex]));
        }
        result.OutputValuesDifference = outputValuesDifference;
        result.Quality = 2d;

        return result;
    }
    protected override double MaxMeterQuality
    {
        get
        {
            return 2d;
        }
    }
}



