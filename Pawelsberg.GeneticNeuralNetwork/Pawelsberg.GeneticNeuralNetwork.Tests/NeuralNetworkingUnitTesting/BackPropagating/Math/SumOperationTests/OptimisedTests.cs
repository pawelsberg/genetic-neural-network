using System.Collections.Generic;
using System.Linq;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.NeuralNetworkingUnitTesting.BackPropagating.Math.SumOperationTests;

public class OptimisedTests
{
    [Fact]
    public void OptimisedDropsZeroConstantWhenOtherTermsExist()
    {
        var input = new Input { InputSynapse = new Synapse(), Value = 3 };
        var sum = new SumOperation
        {
            Expressions = new List<Expression>
            {
                new Constant { Value = 0 },
                input
            }
        };

        var optimised = sum.Optimised();

        Assert.IsType<Input>(optimised);
    }

    [Fact]
    public void OptimisedCombinesConstants()
    {
        var sum = new SumOperation
        {
            Expressions = new List<Expression>
            {
                new Constant { Value = 2 },
                new Constant { Value = 3 }
            }
        };

        var optimised = sum.Optimised();
        var constant = Assert.IsType<Constant>(optimised);
        Assert.Equal(5d, constant.Value);
    }

    [Fact]
    public void OptimisedWithOnlyZeroesReturnsZero()
    {
        var sum = new SumOperation
        {
            Expressions = new List<Expression>
            {
                new Constant { Value = 0 },
                new Constant { Value = 0 }
            }
        };

        var optimised = sum.Optimised();
        var constant = Assert.IsType<Constant>(optimised);
        Assert.Equal(0d, constant.Value);
    }

    [Fact]
    public void OptimisedKeepsSingleNonConstant()
    {
        var input = new Input { InputSynapse = new Synapse(), Value = 3 };
        var sum = new SumOperation { Expressions = new List<Expression> { new Constant { Value = 0 }, input } };

        var optimised = sum.Optimised();

        Assert.IsType<Input>(optimised);
    }

    [Fact]
    public void OptimisedKeepsMultipleNonConstants()
    {
        var sum = new SumOperation
        {
            Expressions = new List<Expression>
            {
                new Input { InputSynapse = new Synapse(), Value = 1 },
                new Multiplier { Synapse = new Synapse(), Value = 2 },
                new Constant { Value = 0 }
            }
        };

        var optimised = sum.Optimised();

        var op = Assert.IsType<SumOperation>(optimised);
        Assert.Equal(2, op.Expressions.Count);
    }

    [Fact]
    public void OptimisedCombinesConstantsAlongsideNonConstants()
    {
        var sum = new SumOperation
        {
            Expressions = new List<Expression>
            {
                new Constant { Value = 2 },
                new Input { InputSynapse = new Synapse(), Value = 1 },
                new Constant { Value = 3 }
            }
        };

        var optimised = sum.Optimised();

        var op = Assert.IsType<SumOperation>(optimised);
        Assert.Equal(2, op.Expressions.Count);
        Assert.Contains(op.Expressions, e => e is Constant c && c.Value == 5);
    }

    [Fact]
    public void OptimisedRemovesZeroConstantFromMany()
    {
        var sum = new SumOperation
        {
            Expressions = new List<Expression>
            {
                new Input { InputSynapse = new Synapse(), Value = 1 },
                new Constant { Value = 0 },
                new Input { InputSynapse = new Synapse(), Value = 2 }
            }
        };

        var optimised = sum.Optimised();
        var op = Assert.IsType<SumOperation>(optimised);
        Assert.Equal(2, op.Expressions.Count);
    }
}
