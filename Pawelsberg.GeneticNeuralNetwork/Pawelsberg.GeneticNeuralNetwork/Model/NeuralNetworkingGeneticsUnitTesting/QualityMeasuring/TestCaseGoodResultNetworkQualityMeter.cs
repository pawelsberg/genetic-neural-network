using System.Globalization;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;

public class TestCaseGoodResultNetworkQualityMeter : QualityMeter<Network>, INetworkQualityMeterTextConvertible
{
    public static string TextName = "GoodResult";
    public TestCaseNetworkQualityMeter TestCaseParent { get { return (TestCaseNetworkQualityMeter)Parent; } }
    public double GoodDifference { get; set; }
    public double QualityForGoodResult { get; set; }

    public TestCaseGoodResultNetworkQualityMeter(QualityMeter<Network> parent, double goodDifference, double qualityForGoodResult) : base(parent)
    {
        GoodDifference = goodDifference;
        QualityForGoodResult = qualityForGoodResult;
    }

    public string ToText() => $"{TextName}({QualityForGoodResult.ToString(CultureInfo.InvariantCulture)},{GoodDifference.ToString(CultureInfo.InvariantCulture)})";

    public static TestCaseGoodResultNetworkQualityMeter Parse(string parameters, QualityMeter<Network> parent)
    {
        string[] parts = CodedText.SplitParams(parameters);
        double qualityForGoodResult = double.Parse(parts[0], CultureInfo.InvariantCulture);
        double goodDiff = double.Parse(parts[1], CultureInfo.InvariantCulture);
        return new TestCaseGoodResultNetworkQualityMeter(parent, goodDiff, qualityForGoodResult);
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



