using System.Collections.Generic;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.NeuralNetworkingUnitTesting.BackPropagating.Math.ActivationDerivativeFunctionOperationTests;

public class OptimisedTests
{
    [Fact]
    public void OptimisedLinearReturnsConstantOne()
    {
        var op = new ActivationDerivativeFunctionOperation
        {
            ActivationFunction = ActivationFunction.Linear,
            Expression = new MultiplyOperation
            {
                Expressions = new List<Expression>
                {
                    new Constant { Value = 1 },
                    new Constant { Value = 2 }
                }
            }
        };

        var optimised = op.Optimised();

        var constant = Assert.IsType<Constant>(optimised);
        Assert.Equal(1d, constant.Value);
    }

    [Fact]
    public void OptimisedKeepsActivationFunctionForNonLinear()
    {
        var op = new ActivationDerivativeFunctionOperation
        {
            ActivationFunction = ActivationFunction.Sigmoid,
            Expression = new Constant { Value = 1 }
        };

        var optimised = op.Optimised();
        var act = Assert.IsType<ActivationDerivativeFunctionOperation>(optimised);
        Assert.Equal(ActivationFunction.Sigmoid, act.ActivationFunction);
    }

    [Fact]
    public void OptimisedClonesExpressionForNonLinear()
    {
        var inner = new Input { InputSynapse = new Synapse(), Value = 2 };
        var op = new ActivationDerivativeFunctionOperation { ActivationFunction = ActivationFunction.Tanh, Expression = inner };

        var optimised = op.Optimised();
        var act = Assert.IsType<ActivationDerivativeFunctionOperation>(optimised);
        Assert.NotSame(inner, act.Expression);
    }

    [Fact]
    public void OptimisedHandlesNestedMultiplicationNonLinear()
    {
        var op = new ActivationDerivativeFunctionOperation
        {
            ActivationFunction = ActivationFunction.Sigmoid,
            Expression = new MultiplyOperation
            {
                Expressions = new List<Expression>
                {
                    new Constant { Value = 2 },
                    new Constant { Value = 3 }
                }
            }
        };

        var optimised = op.Optimised();
        var act = Assert.IsType<ActivationDerivativeFunctionOperation>(optimised);
        Assert.IsType<Constant>(act.Expression);
    }

    [Fact]
    public void OptimisedKeepsValueForNonLinear()
    {
        var op = new ActivationDerivativeFunctionOperation
        {
            ActivationFunction = ActivationFunction.Tanh,
            Expression = new Constant { Value = 0 },
            Value = 42
        };

        var optimised = op.Optimised();
        var act = Assert.IsType<ActivationDerivativeFunctionOperation>(optimised);
        Assert.Equal(42, act.Value);
    }

    [Fact]
    public void OptimisedSupportsComplexInnerNonLinear()
    {
        var inner = new SumOperation
        {
            Expressions = new List<Expression>
            {
                new Constant { Value = 1 },
                new MultiplyOperation
                {
                    Expressions = new List<Expression>
                    {
                        new Constant { Value = 2 },
                        new Constant { Value = 3 }
                    }
                }
            }
        };

        var op = new ActivationDerivativeFunctionOperation
        {
            ActivationFunction = ActivationFunction.Tanh,
            Expression = inner
        };

        var optimised = op.Optimised();
        var act = Assert.IsType<ActivationDerivativeFunctionOperation>(optimised);
        Assert.IsType<Constant>(act.Expression);
    }

    [Fact]
    public void OptimisedLinearIgnoresValueAndInner()
    {
        var op = new ActivationDerivativeFunctionOperation
        {
            ActivationFunction = ActivationFunction.Linear,
            Expression = new Input { InputSynapse = new Synapse(), Value = 5 },
            Value = 99
        };

        var optimised = op.Optimised();

        var constant = Assert.IsType<Constant>(optimised);
        Assert.Equal(1d, constant.Value);
    }
}
