using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.QualityMeasuring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TotalSynapsesNetworkQualityMeterTests;

public class ConstructorTests
{
    [Fact]
    public void SetsQualityForOneSynapse()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        double qualityForOneSynapse = 5.0;

        TotalSynapsesNetworkQualityMeter meter = new TotalSynapsesNetworkQualityMeter(parent, qualityForOneSynapse);

        Assert.Equal(qualityForOneSynapse, meter.QualityForOneSynapse);
    }

    [Fact]
    public void SetsParent()
    {
        QualityMeter<Network> parent = new QualityMeter<Network>(null);
        double qualityForOneSynapse = 5.0;

        TotalSynapsesNetworkQualityMeter meter = new TotalSynapsesNetworkQualityMeter(parent, qualityForOneSynapse);

        Assert.Same(parent, meter.Parent);
    }
}
