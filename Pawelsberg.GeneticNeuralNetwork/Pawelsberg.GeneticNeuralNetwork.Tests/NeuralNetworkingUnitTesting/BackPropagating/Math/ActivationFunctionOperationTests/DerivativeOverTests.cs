using System;
using System.Collections.Generic;
using System.Linq;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.NeuralNetworkingUnitTesting.BackPropagating.Math.ActivationFunctionOperationTests;

public class DerivativeOverTests
{
    [Fact]
    public void DerivativeAppliesChainRule()
    {
        var synapse = new Synapse();
        var input = new Input { InputSynapse = synapse, Value = 2 };
        var multiplier = new Multiplier { Synapse = synapse, Value = 3 };
        var product = new MultiplyOperation { Expressions = new List<Expression> { input, multiplier } };
        var op = new ActivationFunctionOperation
        {
            ActivationFunction = ActivationFunction.Sigmoid,
            Expression = product
        };

        var derivative = op.DerivativeOver(multiplier);
        derivative.CalcValue();

        double z = 2 * 3;
        double sigma = 1d / (1d + System.Math.Exp(-z));
        double expected = sigma * (1 - sigma) * 2; // sigma'(z) * input

        Assert.Equal(expected, derivative.Value, 6);
    }

    [Theory]
    [InlineData(ActivationFunction.Linear, 1d, 1d)]
    [InlineData(ActivationFunction.Tanh, 0.5d, 0.7864477329659274d)]
    [InlineData(ActivationFunction.Squashing, 0.75d, 1d)]
    [InlineData(ActivationFunction.Threshold, 0.2d, 1d)]
    [InlineData(ActivationFunction.Sigmoid, 0d, 0.25d)]
    public void DerivativeUsesActivationDerivative(ActivationFunction fn, double inputVal, double expectedDerivative)
    {
        var synapse = new Synapse();
        var input = new Input { InputSynapse = synapse, Value = inputVal };
        var multiplier = new Multiplier { Synapse = synapse, Value = 1 };
        var product = new MultiplyOperation { Expressions = new List<Expression> { input, multiplier } };
        var op = new ActivationFunctionOperation { ActivationFunction = fn, Expression = product };

        var derivative = op.DerivativeOver(multiplier);
        derivative.CalcValue();

        Assert.Equal(expectedDerivative * inputVal, derivative.Value, 6);
    }

    [Fact]
    public void DerivativeZeroWhenNoTarget()
    {
        var op = new ActivationFunctionOperation
        {
            ActivationFunction = ActivationFunction.Linear,
            Expression = new Constant { Value = 2 }
        };

        var derivative = op.DerivativeOver(new Multiplier { Synapse = new Synapse(), Value = 1 });
        derivative.CalcValue();

        Assert.Equal(0d, derivative.Value);
    }

    [Fact]
    public void DerivativePropagatesThroughNestedMultiplication()
    {
        var synapse = new Synapse();
        var inner = new MultiplyOperation
        {
            Expressions = new List<Expression>
            {
                new Multiplier { Synapse = synapse, Value = 2 },
                new Constant { Value = 4 }
            }
        };

        var op = new ActivationFunctionOperation { ActivationFunction = ActivationFunction.Linear, Expression = inner };

        var derivative = op.DerivativeOver((Multiplier)inner.Expressions[0]);
        derivative.CalcValue();

        Assert.Equal(4d, derivative.Value);
    }

    [Fact]
    public void DerivativeRespectsOptimisedInner()
    {
        var synapse = new Synapse();
        var product = new MultiplyOperation
        {
            Expressions = new List<Expression>
            {
                new Input { InputSynapse = synapse, Value = 3 },
                new Multiplier { Synapse = synapse, Value = 2 }
            }
        };

        var op = new ActivationFunctionOperation { ActivationFunction = ActivationFunction.Linear, Expression = product.Optimised() };

        var derivative = op.DerivativeOver((Multiplier)((MultiplyOperation)op.Expression).Expressions.Last());
        derivative.CalcValue();

        Assert.Equal(3d, derivative.Value);
    }

    [Fact]
    public void OptimisedDerivativeWithZeroInnerDerivativeCollapsesToConstantZero()
    {
        var op = new ActivationFunctionOperation
        {
            ActivationFunction = ActivationFunction.Sigmoid,
            Expression = new Constant { Value = 10 }
        };

        var optimised = op.DerivativeOver(new Multiplier { Synapse = new Synapse(), Value = 5 }).Optimised();

        var constant = Assert.IsType<Constant>(optimised);
        Assert.Equal(0d, constant.Value);
    }

    [Fact]
    public void OptimisedDerivativeDropsMultiplicationByOne()
    {
        var synapse = new Synapse();
        var multiplier = new Multiplier { Synapse = synapse, Value = 1 };
        var op = new ActivationFunctionOperation
        {
            ActivationFunction = ActivationFunction.Tanh,
            Expression = multiplier
        };

        var optimised = op.DerivativeOver(multiplier).Optimised();

        var activationDerivative = Assert.IsType<ActivationDerivativeFunctionOperation>(optimised);
        Assert.Equal(ActivationFunction.Tanh, activationDerivative.ActivationFunction);
        Assert.IsType<Multiplier>(activationDerivative.Expression);
    }

    [Fact]
    public void ChainRuleHandlesNestedActivations()
    {
        var synapse = new Synapse();
        var inner = new ActivationFunctionOperation
        {
            ActivationFunction = ActivationFunction.Sigmoid,
            Expression = new Multiplier { Synapse = synapse, Value = 0.7 }
        };

        var outer = new ActivationFunctionOperation
        {
            ActivationFunction = ActivationFunction.Tanh,
            Expression = inner
        };

        var derivative = outer.DerivativeOver((Multiplier)inner.Expression);
        derivative.CalcValue();

        double sig = 1d / (1d + System.Math.Exp(-0.7d));
        double tanhPrime = 1 - System.Math.Pow(System.Math.Tanh(sig), 2);
        double sigPrime = sig * (1 - sig);

        Assert.Equal(tanhPrime * sigPrime, derivative.Value, 10);
    }

    [Fact]
    public void ChainRuleWithZeroFactorInProductIsZero()
    {
        var synapse = new Synapse();
        var product = new MultiplyOperation
        {
            Expressions = new List<Expression>
            {
                new Constant { Value = 0 },
                new Multiplier { Synapse = synapse, Value = 3 }
            }
        };

        var op = new ActivationFunctionOperation { ActivationFunction = ActivationFunction.Sigmoid, Expression = product };

        var derivative = op.DerivativeOver((Multiplier)product.Expressions[1]).Optimised();
        derivative.CalcValue();

        Assert.Equal(0d, Assert.IsType<Constant>(derivative).Value);
    }

    [Fact]
    public void ChainRuleZeroWhenDifferentTargetSynapse()
    {
        var product = new MultiplyOperation
        {
            Expressions = new List<Expression>
            {
                new Multiplier { Synapse = new Synapse(), Value = 2 },
                new Constant { Value = 5 }
            }
        };

        var op = new ActivationFunctionOperation { ActivationFunction = ActivationFunction.Sigmoid, Expression = product };

        var derivative = op.DerivativeOver(new Multiplier { Synapse = new Synapse(), Value = 1 }).Optimised();
        derivative.CalcValue();

        Assert.Equal(0d, Assert.IsType<Constant>(derivative).Value);
    }

    [Fact]
    public void ChainRuleSumDerivativeOfOneSimplifies()
    {
        var synapse = new Synapse();
        var sum = new SumOperation
        {
            Expressions = new List<Expression>
            {
                new Multiplier { Synapse = synapse, Value = 4 },
                new Constant { Value = 0 }
            }
        };

        var op = new ActivationFunctionOperation { ActivationFunction = ActivationFunction.Linear, Expression = sum };

        var derivative = op.DerivativeOver((Multiplier)sum.Expressions[0]).Optimised();

        var constant = Assert.IsType<Constant>(derivative);
        Assert.Equal(1d, constant.Value, 10);
    }

    [Fact]
    public void ChainRuleSumWithDuplicateTargetsProducesFactorTwo()
    {
        var synapse = new Synapse();
        var sum = new SumOperation
        {
            Expressions = new List<Expression>
            {
                new Multiplier { Synapse = synapse, Value = 2 },
                new Multiplier { Synapse = synapse, Value = 3 }
            }
        };

        var op = new ActivationFunctionOperation { ActivationFunction = ActivationFunction.Linear, Expression = sum };

        var derivative = op.DerivativeOver((Multiplier)sum.Expressions[0]);
        derivative.CalcValue();

        Assert.Equal(2d, derivative.Value, 10);
    }

    [Fact]
    public void ChainRuleProductWithConstantFactorCarriesThrough()
    {
        var synapse = new Synapse();
        var product = new MultiplyOperation
        {
            Expressions = new List<Expression>
            {
                new Constant { Value = 2 },
                new Multiplier { Synapse = synapse, Value = 5 }
            }
        };

        var op = new ActivationFunctionOperation { ActivationFunction = ActivationFunction.Sigmoid, Expression = product };

        var derivative = op.DerivativeOver((Multiplier)product.Expressions[1]);
        derivative.CalcValue();

        double sigma = 1d / (1d + System.Math.Exp(-10d));
        double expected = (sigma * (1 - sigma)) * 2d;

        Assert.Equal(expected, derivative.Value, 10);
    }

    [Fact]
    public void ChainRuleNestedActivationWithSumInner()
    {
        var synapse = new Synapse();
        var inner = new ActivationFunctionOperation
        {
            ActivationFunction = ActivationFunction.Tanh,
            Expression = new SumOperation
            {
                Expressions = new List<Expression>
                {
                    new Multiplier { Synapse = synapse, Value = 1 },
                    new Constant { Value = 1 }
                }
            }
        };

        var outer = new ActivationFunctionOperation { ActivationFunction = ActivationFunction.Sigmoid, Expression = inner };

        var derivative = outer.DerivativeOver((Multiplier)((SumOperation)inner.Expression).Expressions[0]);
        derivative.CalcValue();

        double innerVal = System.Math.Tanh(2d);
        double outerPrime = (1d / (1d + System.Math.Exp(-innerVal))) * (1 - 1d / (1d + System.Math.Exp(-innerVal)));
        double innerPrime = 1 - System.Math.Pow(System.Math.Tanh(2d), 2);

        Assert.Equal(outerPrime * innerPrime, derivative.Value, 10);
    }

    [Fact]
    public void ChainRuleThresholdZeroesDerivativeOutsideRange()
    {
        var synapse = new Synapse();
        var op = new ActivationFunctionOperation
        {
            ActivationFunction = ActivationFunction.Threshold,
            Expression = new Multiplier { Synapse = synapse, Value = 2 }
        };

        var derivative = op.DerivativeOver((Multiplier)op.Expression).Optimised();
        derivative.CalcValue();

        var activationDerivative = Assert.IsType<ActivationDerivativeFunctionOperation>(derivative);
        Assert.Equal(0d, activationDerivative.Value);
    }

    [Fact]
    public void ChainRuleSquashingZeroesDerivativeOutsideRange()
    {
        var synapse = new Synapse();
        var op = new ActivationFunctionOperation
        {
            ActivationFunction = ActivationFunction.Squashing,
            Expression = new Multiplier { Synapse = synapse, Value = -0.5 }
        };

        var derivative = op.DerivativeOver((Multiplier)op.Expression).Optimised();
        derivative.CalcValue();

        var activationDerivative = Assert.IsType<ActivationDerivativeFunctionOperation>(derivative);
        Assert.Equal(0d, activationDerivative.Value);
    }

    [Fact]
    public void ChainRuleHandlesProductWithRepeatedTargets()
    {
        var synapse = new Synapse();
        var product = new MultiplyOperation
        {
            Expressions = new List<Expression>
            {
                new Constant { Value = 3 },
                new Multiplier { Synapse = synapse, Value = 2 },
                new Multiplier { Synapse = synapse, Value = 2 }
            }
        };

        var op = new ActivationFunctionOperation { ActivationFunction = ActivationFunction.Sigmoid, Expression = product };

        var derivative = op.DerivativeOver((Multiplier)product.Expressions[1]);
        derivative.CalcValue();

        double sigma = 1d / (1d + System.Math.Exp(-12d));
        double expectedInnerDerivative = 3d * 2d + 3d * 2d; // const*other + const*first
        double expected = (sigma * (1 - sigma)) * expectedInnerDerivative;

        Assert.Equal(expected, derivative.Value, 8);
    }
}
