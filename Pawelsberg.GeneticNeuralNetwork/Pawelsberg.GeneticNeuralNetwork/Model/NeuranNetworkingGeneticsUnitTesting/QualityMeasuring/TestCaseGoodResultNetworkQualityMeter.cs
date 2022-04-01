using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting.QualityMeasuring;

public class TestCaseGoodResultNetworkQualityMeter : QualityMeter<Network>
{
    public TestCaseNetworkQualityMeter TestCaseParent { get { return (TestCaseNetworkQualityMeter)Parent; } }
    public double GoodDifference { get; set; }
    public double QualityForGoodResult { get; set; }

    public TestCaseGoodResultNetworkQualityMeter(TestCaseNetworkQualityMeter parent, double goodDifference, double qualityForGoodResult) : base(parent)
    {
        GoodDifference = goodDifference;
        QualityForGoodResult = qualityForGoodResult;
    }

    public override QualityMeasurement<Network> MeasureMeterQuality(Network network, QualityMeasurement<Network> parentQualityMeasurement)
    {
        QualityMeasurement<Network> result = new QualityMeasurement<Network>(this, parentQualityMeasurement);

        TestCaseQualityMeasurement parentMeasurement = (TestCaseQualityMeasurement)parentQualityMeasurement;

        double difference = parentMeasurement.OutputValuesDifference;

        if (difference < GoodDifference)
            result.Quality = QualityForGoodResult;
        else
            result.Quality = 0d;

        return result;
    }
    protected override double MaxMeterQuality
    {
        get
        {
            return QualityForGoodResult;
        }
    }
}



