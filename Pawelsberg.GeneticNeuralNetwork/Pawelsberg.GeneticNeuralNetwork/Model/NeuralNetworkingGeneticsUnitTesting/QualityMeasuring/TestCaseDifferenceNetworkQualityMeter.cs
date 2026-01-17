using System.Globalization;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;

public class TestCaseDifferenceNetworkQualityMeter : QualityMeter<Network>, INetworkQualityMeterTextConvertible
{
    public static string TextName = "Difference";
    public TestCaseNetworkQualityMeter TestCaseParent { get { return (TestCaseNetworkQualityMeter)Parent; } }
    public double GoodDifference { get; set; }
    public double QualityForOneDiff { get; set; }

    public TestCaseDifferenceNetworkQualityMeter(QualityMeter<Network> parent, double goodDifference, double qualityForOneDiff) : base(parent)
    {
        GoodDifference = goodDifference;
        QualityForOneDiff = qualityForOneDiff;
    }

    public string ToText() => $"{TextName}({QualityForOneDiff.ToString(CultureInfo.InvariantCulture)},{GoodDifference.ToString(CultureInfo.InvariantCulture)})";

    public static TestCaseDifferenceNetworkQualityMeter Parse(string parameters, QualityMeter<Network> parent)
    {
        string[] parts = CodedText.SplitParams(parameters);
        double qualityForOneDiff = double.Parse(parts[0], CultureInfo.InvariantCulture);
        double goodDiff = double.Parse(parts[1], CultureInfo.InvariantCulture);
        return new TestCaseDifferenceNetworkQualityMeter(parent, goodDiff, qualityForOneDiff);
    }

    public override QualityMeasurement<Network> MeasureMeterQuality(Network network, QualityMeasurement<Network> parentQualityMeasurement)
    {
        QualityMeasurement<Network> result = new QualityMeasurement<Network>(this, parentQualityMeasurement);
        TestCaseQualityMeasurement parentMeasurement = (TestCaseQualityMeasurement)parentQualityMeasurement;

        double difference = parentMeasurement.OutputValuesDifference;

        if (difference < GoodDifference)
            result.Quality = QualityForOneDiff / GoodDifference;
        else
            result.Quality = QualityForOneDiff / difference;

        return result;
    }
    protected override double MaxMeterQuality
    {
        get
        {
            return QualityForOneDiff / GoodDifference;
        }
    }
}

