using System.Globalization;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.QualityMeasuring;

public class NoLoopsNetworkQualityMeter : QualityMeter<Network>, INetworkQualityMeterTextConvertible
{
    public static string TextName = "NoLoops";
    public double QualityForZeroLoops { get; set; }

    public NoLoopsNetworkQualityMeter(QualityMeter<Network> parent, double qualityForZeroLoops) : base(parent)
    {
        QualityForZeroLoops = qualityForZeroLoops;
    }

    public string ToText() => $"{TextName}({QualityForZeroLoops.ToString(CultureInfo.InvariantCulture)})";

    public override QualityMeasurement<Network> MeasureMeterQuality(Network network, QualityMeasurement<Network> parentQualityMeasurement)
    {
        QualityMeasurement<Network> result = new QualityMeasurement<Network>(this, parentQualityMeasurement);

        bool loopExists = false;
        foreach (Synapse networkInput in network.Inputs)
        {
            List<Node> traversedNodes = new List<Node>();
            loopExists = LoopExists(networkInput, traversedNodes, network);
            if (loopExists)
                break;
        }
        result.Quality += loopExists ? 0 : QualityForZeroLoops;
        return result;
    }

    private bool LoopExists(Synapse synapse, List<Node> traversedNodes, Network network)
    {
        if (network.Outputs.Contains(synapse))
            return false;

        Neuron neuron = network.Nodes.Single(n => n is Neuron && ((Neuron)n).Inputs.Contains(synapse)) as Neuron;

        if (neuron == null)
            throw new Exception("Synapse is not an network output but it is not an input for an neuron either.");
        if (traversedNodes.Contains(neuron))
            return true;

        List<Node> newTraversedNodes = new List<Node>(traversedNodes);
        newTraversedNodes.Add(neuron);

        foreach (Synapse neuronOutput in neuron.Outputs)
        {
            bool loopExists = LoopExists(neuronOutput, newTraversedNodes, network);
            if (loopExists)
                return true;
        }

        return false;
    }

    protected override double MaxMeterQuality
    {
        get
        {
            return QualityForZeroLoops;
        }
    }
}
