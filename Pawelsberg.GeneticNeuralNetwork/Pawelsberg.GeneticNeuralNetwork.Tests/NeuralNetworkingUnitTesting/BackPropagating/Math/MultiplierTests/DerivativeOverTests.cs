using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.NeuralNetworkingUnitTesting.BackPropagating.Math.MultiplierTests;

public class DerivativeOverTests
{
    [Theory]
    [InlineData(1d, 1d, true, 1d)]
    [InlineData(1d, 2d, true, 1d)]
    [InlineData(5d, 7d, false, 0d)]
    [InlineData(-1d, 0d, false, 0d)]
    [InlineData(0d, 0d, true, 1d)]
    public void DerivativeMatchesSynapseEquality(double valueA, double valueB, bool sameSynapse, double expected)
    {
        var synapse = new Synapse();
        var other = sameSynapse ? synapse : new Synapse();

        var multiplier = new Multiplier { Synapse = synapse, Value = valueA };
        var target = new Multiplier { Synapse = other, Value = valueB };

        var derivative = multiplier.DerivativeOver(target);
        var result = Assert.IsType<Constant>(derivative);

        Assert.Equal(expected, result.Value);
    }
}
