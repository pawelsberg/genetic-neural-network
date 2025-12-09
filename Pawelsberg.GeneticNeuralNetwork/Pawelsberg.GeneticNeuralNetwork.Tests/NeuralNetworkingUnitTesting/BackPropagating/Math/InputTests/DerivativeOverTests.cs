using System;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.NeuralNetworkingUnitTesting.BackPropagating.Math.InputTests;

public class DerivativeOverTests
{
    [Theory]
    [InlineData(0d)]
    [InlineData(1d)]
    [InlineData(-1d)]
    [InlineData(10d)]
    [InlineData(double.PositiveInfinity)]
    public void InputDerivativeAlwaysZero(double val)
    {
        var target = new Multiplier { Synapse = new Synapse(), Value = 1 };
        var input = new Input { InputSynapse = target.Synapse, Value = val };

        var derivative = input.DerivativeOver(target);

        var result = Assert.IsType<Constant>(derivative);
        Assert.Equal(0d, result.Value);
    }

    [Fact]
    public void DerivativeZeroForDifferentSynapse()
    {
        var input = new Input { InputSynapse = new Synapse(), Value = 3 };
        var target = new Multiplier { Synapse = new Synapse(), Value = 2 };

        var derivative = input.DerivativeOver(target);

        var result = Assert.IsType<Constant>(derivative);
        Assert.Equal(0d, result.Value);
    }

    [Fact]
    public void DerivativeZeroWhenTargetSharesNoReference()
    {
        var synapseA = new Synapse();
        var synapseB = new Synapse();
        var input = new Input { InputSynapse = synapseA, Value = -10 };
        var target = new Multiplier { Synapse = synapseB, Value = 99 };

        var derivative = input.DerivativeOver(target);

        Assert.Equal(0d, Assert.IsType<Constant>(derivative).Value);
    }

    [Fact]
    public void DerivativeZeroRegardlessOfMultiplierValue()
    {
        var synapse = new Synapse();
        var input = new Input { InputSynapse = synapse, Value = double.NaN };
        var target = new Multiplier { Synapse = synapse, Value = -1234.5 };

        var derivative = input.DerivativeOver(target);

        Assert.Equal(0d, Assert.IsType<Constant>(derivative).Value);
    }
}
