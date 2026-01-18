using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;

public static class NetworkQualityMeterExtension
{
    public static void Configure(this QualityMeter<Network> meter, int propagations, TestCaseList testCaseList)
    {
        if (meter is INetworkQualityMeterWithPropagations withPropagations)
            withPropagations.Propagations = propagations;

        if (meter is INetworkQualityMeterWithTestCaseList withTestCaseList)
            withTestCaseList.TestCaseList = testCaseList;

        List<QualityMeter<Network>>? children = meter.Children;
        if (children != null)
            foreach (QualityMeter<Network> child in children)
                child.Configure(propagations, testCaseList);
    }
}
