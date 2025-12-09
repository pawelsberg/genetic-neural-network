using System.Collections.Generic;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.NeuralNetworkingUnitTesting.BackPropagating.Math.ActivationFunctionOperationTests;

public class OptimisedTests
{
    [Fact]
    public void OptimisedWrapsOptimisedInner()
    {
        var inner = new MultiplyOperation
        {
            Expressions = new List<Expression>
            {
                new Constant { Value = 1 },
                new Constant { Value = 2 }
            }
        };

        var op = new ActivationFunctionOperation
        {
            ActivationFunction = ActivationFunction.Linear,
            Expression = inner
        };

        var optimised = op.Optimised();

        var act = Assert.IsType<ActivationFunctionOperation>(optimised);
        Assert.IsType<Constant>(act.Expression);
    }

    [Fact]
    public void OptimisedKeepsActivationFunction()
    {
        var op = new ActivationFunctionOperation
        {
            ActivationFunction = ActivationFunction.Tanh,
            Expression = new Constant { Value = 1 }
        };

        var optimised = op.Optimised();
        var act = Assert.IsType<ActivationFunctionOperation>(optimised);
        Assert.Equal(ActivationFunction.Tanh, act.ActivationFunction);
    }

    [Fact]
    public void OptimisedClonesInner()
    {
        var inner = new Input { InputSynapse = new Synapse(), Value = 2 };
        var op = new ActivationFunctionOperation { ActivationFunction = ActivationFunction.Linear, Expression = inner };

        var optimised = op.Optimised();
        var act = Assert.IsType<ActivationFunctionOperation>(optimised);
        Assert.NotSame(inner, act.Expression);
    }

    [Fact]
    public void OptimisedReducesNestedOptimisations()
    {
        var inner = new MultiplyOperation
        {
            Expressions = new List<Expression>
            {
                new Constant { Value = 0 },
                new Constant { Value = 5 }
            }
        };

        var op = new ActivationFunctionOperation { ActivationFunction = ActivationFunction.Linear, Expression = inner };

        var optimised = op.Optimised();
        var act = Assert.IsType<ActivationFunctionOperation>(optimised);
        Assert.IsType<Constant>(act.Expression);
    }

    [Fact]
    public void OptimisedLeavesValueUnchanged()
    {
        var op = new ActivationFunctionOperation
        {
            ActivationFunction = ActivationFunction.Linear,
            Expression = new Constant { Value = 3 },
            Value = 9
        };

        var optimised = op.Optimised();
        var act = Assert.IsType<ActivationFunctionOperation>(optimised);
        Assert.Equal(9, act.Value);
    }

    [Fact]
    public void OptimisedHandlesDeepInnerExpression()
    {
        var deep = new SumOperation
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

        var op = new ActivationFunctionOperation { ActivationFunction = ActivationFunction.Linear, Expression = deep };

        var optimised = op.Optimised();
        var act = Assert.IsType<ActivationFunctionOperation>(optimised);
        Assert.IsType<Constant>(act.Expression);
    }
}
