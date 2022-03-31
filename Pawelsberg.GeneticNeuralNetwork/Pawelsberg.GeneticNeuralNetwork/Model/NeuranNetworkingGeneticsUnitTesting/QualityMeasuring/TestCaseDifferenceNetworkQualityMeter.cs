using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting.QualityMeasuring
{

    public class TestCaseDifferenceNetworkQualityMeter : QualityMeter<Network>
    {
        public TestCaseNetworkQualityMeter TestCaseParent { get { return (TestCaseNetworkQualityMeter)Parent; } }
        public double GoodDifference { get; set; }
        public double QualityForOneDiff { get; set; }

        public TestCaseDifferenceNetworkQualityMeter(TestCaseNetworkQualityMeter parent, double goodDifference, double qualityForOneDiff) : base(parent)
        {
            GoodDifference = goodDifference;
            QualityForOneDiff = qualityForOneDiff;
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

}
