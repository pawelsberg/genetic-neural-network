using System.Collections.Generic;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.NeuralNetworkingUnitTesting.BackPropagating.Math.MultiplyOperationTests;

public class DerivativeOverTests
{
    [Fact]
    public void DerivativeAppliesProductRule()
    {
        var synapse = new Synapse();
        var input = new Input { InputSynapse = synapse, Value = 2 };
        var multiplier = new Multiplier { Synapse = synapse, Value = 3 };
        var other = new Constant { Value = 5 };

        var product = new MultiplyOperation { Expressions = new List<Expression> { input, multiplier, other } };

        var derivative = product.DerivativeOver(multiplier);
        derivative.CalcValue();

        Assert.Equal(10d, derivative.Value); // derivative = input * other = 2*5
    }

    [Fact]
    public void DerivativeSingleFactorDelegates()
    {
        var target = new Multiplier { Synapse = new Synapse(), Value = 1 };
        var product = new MultiplyOperation { Expressions = new List<Expression> { target } };

        var derivative = product.DerivativeOver(target);

        var result = Assert.IsType<Constant>(derivative);
        Assert.Equal(1d, result.Value);
    }

    [Fact]
    public void DerivativeTwoFactorsWorks()
    {
        var synapse = new Synapse();
        var input = new Input { InputSynapse = synapse, Value = 4 };
        var multiplier = new Multiplier { Synapse = synapse, Value = 2 };

        var product = new MultiplyOperation { Expressions = new List<Expression> { input, multiplier } };

        var derivative = product.DerivativeOver(multiplier);
        derivative.CalcValue();

        Assert.Equal(4d, derivative.Value);
    }

    [Fact]
    public void DerivativeIgnoresConstants()
    {
        var synapse = new Synapse();
        var multiplier = new Multiplier { Synapse = synapse, Value = 2 };

        var product = new MultiplyOperation
        {
            Expressions = new List<Expression>
            {
                new Constant { Value = 3 },
                new Constant { Value = 4 },
                multiplier
            }
        };

        var derivative = product.DerivativeOver(multiplier);
        derivative.CalcValue();

        Assert.Equal(12d, derivative.Value);
    }

    [Fact]
    public void DerivativeWithNestedProductsFlattens()
    {
        var synapse = new Synapse();
        var multiplier = new Multiplier { Synapse = synapse, Value = 2 };
        var inner = new MultiplyOperation
        {
            Expressions = new List<Expression>
            {
                multiplier,
                new Constant { Value = 3 }
            }
        };

        var product = new MultiplyOperation
        {
            Expressions = new List<Expression>
            {
                new Constant { Value = 2 },
                inner
            }
        };

        var derivative = product.DerivativeOver(multiplier);
        derivative.CalcValue();

        Assert.Equal(6d, derivative.Value);
    }

    [Fact]
    public void DerivativeWithNoTargetMultiplierIsZero()
    {
        var product = new MultiplyOperation
        {
            Expressions = new List<Expression>
            {
                new Constant { Value = 2 },
                new Input { InputSynapse = new Synapse(), Value = 3 }
            }
        };

        var derivative = product.DerivativeOver(new Multiplier { Synapse = new Synapse(), Value = 1 });
        derivative.CalcValue();

        Assert.Equal(0d, derivative.Value);
    }
}
