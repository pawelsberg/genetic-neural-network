using System.Globalization;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting.QualityMeasuring;

public class TestCaseSubstractErrorIfGoodNetworkQualityMeter : QualityMeter<Network>, INetworkQualityMeterTextConvertible
{
    public static string TextName = "SubstractErrorIfGood";
    public TestCaseNetworkQualityMeter TestCaseParent { get { return (TestCaseNetworkQualityMeter)Parent; } }
    public double GoodDifference { get; set; }
    public double SubstractedQualityForOneDiff { get; set; }

    public TestCaseSubstractErrorIfGoodNetworkQualityMeter(QualityMeter<Network> parent, double goodDifference, double substractedQualityForOneDiff) : base(parent)
    {
        GoodDifference = goodDifference;
        SubstractedQualityForOneDiff = substractedQualityForOneDiff;
    }

    public string ToText() => $"{TextName}({SubstractedQualityForOneDiff.ToString(CultureInfo.InvariantCulture)},{GoodDifference.ToString(CultureInfo.InvariantCulture)})";

    public override QualityMeasurement<Network> MeasureMeterQuality(Network network, QualityMeasurement<Network> parentQualityMeasurement)
    {
        QualityMeasurement<Network> result = new QualityMeasurement<Network>(this, parentQualityMeasurement);

        TestCaseQualityMeasurement parentMeasurement = (TestCaseQualityMeasurement)parentQualityMeasurement;

        double difference = parentMeasurement.OutputValuesDifference;

        if (difference < GoodDifference)
            result.Quality = -SubstractedQualityForOneDiff * difference;
        else
            result.Quality = 0d;

        return result;
    }
    protected override double MaxMeterQuality
    {
        get
        {
            return 0d;
        }
    }
}


