using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting.QualityMeasuring
{

    public class TestCasesAllGoodQualityMeasurement : QualityMeasurement<Network>
    {
        public bool AllGood { get; set; }
        public TestCasesAllGoodQualityMeasurement(TestCasesIfAllGoodNetworkQualityMeter meter, QualityMeasurement<Network> parent) : base(meter, parent) { }
    }

}
