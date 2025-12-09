using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.NeuralNetworkingUnitTesting.BackPropagating.Math.MultiplierTests;

public class ToStringTests
{
    [Fact]
    public void ToStringFormatsWithValue()
    {
        var multiplier = new Multiplier { Synapse = new Synapse(), Value = 1.25 };

        Assert.Equal("(M:1.25)", multiplier.ToString());
    }
}
