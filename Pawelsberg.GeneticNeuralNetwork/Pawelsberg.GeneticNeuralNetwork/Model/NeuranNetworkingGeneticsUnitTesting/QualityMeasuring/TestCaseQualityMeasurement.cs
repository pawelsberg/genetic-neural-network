using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting.QualityMeasuring
{
    public class TestCaseQualityMeasurement : QualityMeasurement<Network>
    {
        public TimeSpan RunningTime { get; set; }
        public double OutputValuesDifference { get; set; }
        public TestCaseQualityMeasurement(TestCaseNetworkQualityMeter meter, QualityMeasurement<Network> parent) : base(meter, parent) { }
    }
}
