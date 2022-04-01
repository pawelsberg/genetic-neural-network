using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.QualityMeasuring;

public class TotalNodesNetworkQualityMeter : QualityMeter<Network>
{
    public double QualityForOneNode { get; set; }

    public TotalNodesNetworkQualityMeter(QualityMeter<Network> parent, double qualityForOneNode) : base(parent)
    {
        QualityForOneNode = qualityForOneNode;
    }

    public override QualityMeasurement<Network> MeasureMeterQuality(Network network, QualityMeasurement<Network> parentQualityMeasurement)
    {
        QualityMeasurement<Network> result = new QualityMeasurement<Network>(this, parentQualityMeasurement);
        result.Quality += QualityForOneNode / network.Nodes.Count; // less nodes then better
        return result;
    }
    protected override double MaxMeterQuality
    {
        get
        {
            return QualityForOneNode;
        }
    }
}
