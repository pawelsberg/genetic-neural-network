using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.NeuralNetworkingUnitTesting.BackPropagating.Math.InputTests;

public class ToStringTests
{
    [Fact]
    public void ToStringFormatsWithValue()
    {
        var input = new Input { InputSynapse = new Synapse(), Value = 5 };

        Assert.Equal("(I:5)", input.ToString());
    }
}
