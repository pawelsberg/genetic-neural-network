using System;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.NeuralNetworkingUnitTesting.BackPropagating.Math.ConstantTests;

public class DerivativeOverTests
{
    [Theory]
    [InlineData(0d)]
    [InlineData(2.5d)]
    [InlineData(-3.7d)]
    [InlineData(100d)]
    [InlineData(double.NaN)]
    public void ConstantDerivativeIsAlwaysZero(double value)
    {
        var target = new Multiplier { Synapse = new Synapse(), Value = 1 };
        var constant = new Constant { Value = value };

        var derivative = constant.DerivativeOver(target);

        var result = Assert.IsType<Constant>(derivative);
        Assert.Equal(0d, result.Value);
    }

    [Fact]
    public void DerivativeIgnoresTargetValue()
    {
        var target = new Multiplier { Synapse = new Synapse(), Value = -999 };
        var constant = new Constant { Value = 12 };

        var derivative = constant.DerivativeOver(target);

        Assert.Equal(0d, Assert.IsType<Constant>(derivative).Value);
    }

    [Fact]
    public void DerivativeProducesNewInstance()
    {
        var target = new Multiplier { Synapse = new Synapse(), Value = 1 };
        var constant = new Constant { Value = 1 };

        var derivative = constant.DerivativeOver(target);

        var result = Assert.IsType<Constant>(derivative);
        Assert.NotSame(constant, result);
    }

    [Fact]
    public void DerivativeWithDifferentSynapseStillZero()
    {
        var target = new Multiplier { Synapse = new Synapse(), Value = 1 };
        var constant = new Constant { Value = -5 };

        var derivative = constant.DerivativeOver(target);

        Assert.Equal(0d, Assert.IsType<Constant>(derivative).Value);
    }
}
