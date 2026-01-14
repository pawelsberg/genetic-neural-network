using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TestCasesTotalTimeNetworkQualityMeterTests;

public class ConstructorTests
{
    [Fact]
    public void SetsQualityForOneMs()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        double qualityForOneMs = 100.0;

        TestCasesTotalTimeNetworkQualityMeter meter = new TestCasesTotalTimeNetworkQualityMeter(parent, qualityForOneMs);

        Assert.Equal(qualityForOneMs, meter.QualityForOneMs);
    }

    [Fact]
    public void SetsParent()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        double qualityForOneMs = 100.0;

        TestCasesTotalTimeNetworkQualityMeter meter = new TestCasesTotalTimeNetworkQualityMeter(parent, qualityForOneMs);

        Assert.Same(parent, meter.Parent);
    }
}
