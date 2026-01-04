using System.Globalization;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.QualityMeasuring;

public class TotalNodesNetworkQualityMeter : QualityMeter<Network>, INetworkQualityMeterTextConvertible
{
    public static string TextName = "TotalNodes";
    public double QualityForOneNode { get; set; }

    public TotalNodesNetworkQualityMeter(QualityMeter<Network> parent, double qualityForOneNode) : base(parent)
    {
        QualityForOneNode = qualityForOneNode;
    }

    public string ToText() => $"{TextName}({QualityForOneNode.ToString(CultureInfo.InvariantCulture)})";

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
