using System.Collections.Generic;
using System.Linq;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.NeuralNetworkingUnitTesting.BackPropagating.Math.MultiplyOperationTests;

public class OptimisedTests
{
    [Fact]
    public void OptimisedWithZeroReturnsConstantZero()
    {
        var expr = new MultiplyOperation
        {
            Expressions = new List<Expression>
            {
                new Constant { Value = 0 },
                new Input { InputSynapse = new Synapse(), Value = 3 }
            }
        };

        var optimised = expr.Optimised();

        var constant = Assert.IsType<Constant>(optimised);
        Assert.Equal(0d, constant.Value);
    }

    [Fact]
    public void OptimisedCombinesConstants()
    {
        var expr = new MultiplyOperation
        {
            Expressions = new List<Expression>
            {
                new Constant { Value = 2 },
                new Constant { Value = 3 },
                new Input { InputSynapse = new Synapse(), Value = 5 }
            }
        };

        var optimised = expr.Optimised();
        var multiply = Assert.IsType<MultiplyOperation>(optimised);

        Assert.Single(multiply.Expressions.OfType<Constant>());
        Assert.Equal(6d, multiply.Expressions.OfType<Constant>().First().Value);
    }

    [Fact]
    public void OptimisedRemovesMultiplicationByOne()
    {
        var expr = new MultiplyOperation
        {
            Expressions = new List<Expression>
            {
                new Constant { Value = 1 },
                new Input { InputSynapse = new Synapse(), Value = 5 }
            }
        };

        var optimised = expr.Optimised();

        Assert.IsType<Input>(optimised);
    }

    [Fact]
    public void OptimisedCollapsesAllConstants()
    {
        var expr = new MultiplyOperation
        {
            Expressions = new List<Expression>
            {
                new Constant { Value = 2 },
                new Constant { Value = 4 }
            }
        };

        var optimised = expr.Optimised();

        var constant = Assert.IsType<Constant>(optimised);
        Assert.Equal(8d, constant.Value);
    }

    [Fact]
    public void OptimisedPreservesMultipleNonConstants()
    {
        var expr = new MultiplyOperation
        {
            Expressions = new List<Expression>
            {
                new Input { InputSynapse = new Synapse(), Value = 2 },
                new Multiplier { Synapse = new Synapse(), Value = 3 }
            }
        };

        var optimised = expr.Optimised();

        var multiply = Assert.IsType<MultiplyOperation>(optimised);
        Assert.Equal(2, multiply.Expressions.Count);
    }

    [Fact]
    public void OptimisedMovesConstantToEnd()
    {
        var expr = new MultiplyOperation
        {
            Expressions = new List<Expression>
            {
                new Constant { Value = 2 },
                new Input { InputSynapse = new Synapse(), Value = 3 }
            }
        };

        var optimised = expr.Optimised();

        var multiply = Assert.IsType<MultiplyOperation>(optimised);
        Assert.IsType<Constant>(multiply.Expressions.Last());
    }

    [Fact]
    public void OptimisedNestedMultiplyIsFlattened()
    {
        var nested = new MultiplyOperation
        {
            Expressions = new List<Expression>
            {
                new Constant { Value = 2 },
                new Constant { Value = 3 }
            }
        };

        var expr = new MultiplyOperation
        {
            Expressions = new List<Expression>
            {
                nested,
                new Input { InputSynapse = new Synapse(), Value = 5 }
            }
        };

        var optimised = expr.Optimised();

        var multiply = Assert.IsType<MultiplyOperation>(optimised);
        Assert.Equal(2, multiply.Expressions.Count);
    }
}
