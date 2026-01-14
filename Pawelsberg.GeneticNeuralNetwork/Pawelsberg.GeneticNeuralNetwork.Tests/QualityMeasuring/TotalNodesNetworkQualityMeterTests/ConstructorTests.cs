using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.QualityMeasuring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TotalNodesNetworkQualityMeterTests;

public class ConstructorTests
{
    [Fact]
    public void SetsQualityForOneNode()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        double qualityForOneNode = 10.0;

        TotalNodesNetworkQualityMeter meter = new TotalNodesNetworkQualityMeter(parent, qualityForOneNode);

        Assert.Equal(qualityForOneNode, meter.QualityForOneNode);
    }

    [Fact]
    public void SetsParent()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        double qualityForOneNode = 10.0;

        TotalNodesNetworkQualityMeter meter = new TotalNodesNetworkQualityMeter(parent, qualityForOneNode);

        Assert.Same(parent, meter.Parent);
    }
}
