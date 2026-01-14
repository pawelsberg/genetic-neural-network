using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.QualityMeasuring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.NoLoopsNetworkQualityMeterTests;

public class ConstructorTests
{
    [Fact]
    public void SetsQualityForZeroLoops()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        double qualityForZeroLoops = 50.0;

        NoLoopsNetworkQualityMeter meter = new NoLoopsNetworkQualityMeter(parent, qualityForZeroLoops);

        Assert.Equal(qualityForZeroLoops, meter.QualityForZeroLoops);
    }

    [Fact]
    public void SetsParent()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        double qualityForZeroLoops = 50.0;

        NoLoopsNetworkQualityMeter meter = new NoLoopsNetworkQualityMeter(parent, qualityForZeroLoops);

        Assert.Same(parent, meter.Parent);
    }
}
