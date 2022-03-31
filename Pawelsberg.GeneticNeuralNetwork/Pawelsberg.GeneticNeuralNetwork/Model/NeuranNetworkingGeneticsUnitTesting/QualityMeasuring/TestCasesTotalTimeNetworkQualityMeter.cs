using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting.QualityMeasuring
{

    public class TestCasesTotalTimeNetworkQualityMeter : QualityMeter<Network>
    {
        public double QualityForOneMs { get; set; }

        public TestCasesTotalTimeNetworkQualityMeter(QualityMeter<Network> parent, double qualityForOneMs) : base(parent)
        {
            QualityForOneMs = qualityForOneMs;
        }

        public override QualityMeasurement<Network> MeasureMeterQuality(Network network, QualityMeasurement<Network> parentQualityMeasurement)
        {
            QualityMeasurement<Network> result = new QualityMeasurement<Network>(this, parentQualityMeasurement);

            IEnumerable<TestCaseQualityMeasurement> qualityMeasurements = parentQualityMeasurement.Children.Cast<TestCaseQualityMeasurement>();

            TimeSpan timeSpan = TimeSpan.Zero;

            foreach (TestCaseQualityMeasurement tcMeasurement in qualityMeasurements)
            {
                timeSpan.Add(tcMeasurement.RunningTime);
            }

            result.Quality = QualityForOneMs / (timeSpan.TotalMilliseconds + 1);

            return result;
        }
        protected override double MaxMeterQuality
        {
            get
            {
                return QualityForOneMs;
            }
        }
    }

}
