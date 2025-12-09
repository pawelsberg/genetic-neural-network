using System.Collections.Generic;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.NeuralNetworkingUnitTesting.BackPropagating.Math.ExpressionTests;

public class OptimisedTests
{
    [Fact]
    public void DefaultOptimisedDeepClones()
    {
        var constant = new Constant { Value = 4 };

        var optimised = constant.Optimised();

        Assert.IsType<Constant>(optimised);
        Assert.NotSame(constant, optimised);
    }

    [Fact]
    public void OptimisedOnInputCreatesNewInstance()
    {
        var input = new Input { InputSynapse = new Synapse(), Value = 1 };

        var optimised = input.Optimised();

        Assert.IsType<Input>(optimised);
        Assert.NotSame(input, optimised);
    }

    [Fact]
    public void OptimisedOnMultiplierCreatesNewInstance()
    {
        var multiplier = new Multiplier { Synapse = new Synapse(), Value = 2 };

        var optimised = multiplier.Optimised();

        Assert.IsType<Multiplier>(optimised);
        Assert.NotSame(multiplier, optimised);
    }

    [Fact]
    public void OptimisedKeepsValue()
    {
        var constant = new Constant { Value = 7 };

        var optimised = constant.Optimised();

        Assert.Equal(constant.Value, ((Constant)optimised).Value);
    }

    [Fact]
    public void OptimisedDeepCloneDoesNotShareChild()
    {
        var inner = new Constant { Value = 1 };
        var wrapper = new SumOperation { Expressions = new List<Expression> { inner } };

        var optimised = wrapper.Optimised();

        var constant = Assert.IsType<Constant>(optimised);
        Assert.NotSame(inner, constant);
    }

    [Fact]
    public void OptimisedOnSumReturnsEquivalentStructure()
    {
        var sum = new SumOperation
        {
            Expressions = new List<Expression>
            {
                new Constant { Value = 1 },
                new Input { InputSynapse = new Synapse(), Value = 2 }
            }
        };

        var optimised = sum.Optimised();

        Assert.IsType<SumOperation>(optimised);
    }
}
