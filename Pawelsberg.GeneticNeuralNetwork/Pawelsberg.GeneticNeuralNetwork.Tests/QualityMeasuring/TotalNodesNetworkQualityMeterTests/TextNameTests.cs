using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.QualityMeasuring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.QualityMeasuring.TotalNodesNetworkQualityMeterTests;

public class TextNameTests
{
    [Fact]
    public void IsTotalNodes()
    {
        Assert.Equal("TotalNodes", TotalNodesNetworkQualityMeter.TextName);
    }
}
