using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting.QualityMeasuring
{
    public class TestCasesIfAllGoodNetworkQualityMeter : QualityMeter<Network>
    {
        public double GoodDifference { get; set; }

        public TestCasesIfAllGoodNetworkQualityMeter(QualityMeter<Network> parent, double goodDifference) : base(parent)
        {
            GoodDifference = goodDifference;
        }

        public override QualityMeasurement<Network> MeasureMeterQuality(Network network, QualityMeasurement<Network> parentQualityMeasurement)
        {
            TestCasesAllGoodQualityMeasurement result = new TestCasesAllGoodQualityMeasurement(this, parentQualityMeasurement);

            IEnumerable<TestCaseQualityMeasurement> qualityMeasurements = parentQualityMeasurement.Children.Where(qm => qm is TestCaseQualityMeasurement).Cast<TestCaseQualityMeasurement>();

            foreach (TestCaseQualityMeasurement tcMeasurement in qualityMeasurements)
            {
                if (tcMeasurement.OutputValuesDifference >= GoodDifference)
                {
                    result.AllGood = false;
                    return result;
                }
            }

            result.AllGood = true;
            return result;
        }

        public override QualityMeasurement<Network> MeasureQualityRecursive(Network network, QualityMeasurement<Network> parentQualityMeasurement)
        {
            TestCasesAllGoodQualityMeasurement qualityMeasurement = (TestCasesAllGoodQualityMeasurement)MeasureMeterQuality(network, parentQualityMeasurement);
            if (qualityMeasurement.AllGood)
            {
                foreach (QualityMeter<Network> childMeter in Children)
                {
                    QualityMeasurement<Network> childQualityMeasurement = childMeter.MeasureQualityRecursive(network, qualityMeasurement);
                    qualityMeasurement.Children.Add(childQualityMeasurement);
                    qualityMeasurement.Quality += childQualityMeasurement.Quality;
                }
            }
            return qualityMeasurement;
        }


        protected override double MaxMeterQuality
        {
            get
            {
                return 0d;
            }
        }
    }




}
