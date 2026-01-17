using System.Globalization;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.QualityMeasuring;

public class TotalSynapsesNetworkQualityMeter : QualityMeter<Network>, INetworkQualityMeterTextConvertible
{
    public static string TextName = "TotalSynapses";
    public double QualityForOneSynapse { get; set; }

    public TotalSynapsesNetworkQualityMeter(QualityMeter<Network> parent, double qualityForOneSynapse) : base(parent)
    {
        QualityForOneSynapse = qualityForOneSynapse;
    }

    public string ToText() => $"{TextName}({QualityForOneSynapse.ToString(CultureInfo.InvariantCulture)})";

    public static TotalSynapsesNetworkQualityMeter Parse(string parameters, QualityMeter<Network> parent)
    {
        double qualityForOneSynapse = double.Parse(parameters, CultureInfo.InvariantCulture);
        return new TotalSynapsesNetworkQualityMeter(parent, qualityForOneSynapse);
    }

    public override QualityMeasurement<Network> MeasureMeterQuality(Network network, QualityMeasurement<Network> parentQualityMeasurement)
    {
        QualityMeasurement<Network> result = new QualityMeasurement<Network>(this, parentQualityMeasurement);
        result.Quality += QualityForOneSynapse / network.SynapseCount(); // less synapses then better
        return result;
    }
    protected override double MaxMeterQuality
    {
        get
        {
            return QualityForOneSynapse;
        }
    }
}


