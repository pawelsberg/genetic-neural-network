using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.QualityMeasuring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.MultiplierSumNetworkQualityMeterTests;

public class ConstructorTests
{
    [Fact]
    public void SetsQualityForSumEqOne()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        double qualityForSumEqOne = 20.0;

        MultiplierSumNetworkQualityMeter meter = new MultiplierSumNetworkQualityMeter(parent, qualityForSumEqOne);

        Assert.Equal(qualityForSumEqOne, meter.QualityForSumEqOne);
    }

    [Fact]
    public void SetsParent()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        double qualityForSumEqOne = 20.0;

        MultiplierSumNetworkQualityMeter meter = new MultiplierSumNetworkQualityMeter(parent, qualityForSumEqOne);

        Assert.Same(parent, meter.Parent);
    }
}
