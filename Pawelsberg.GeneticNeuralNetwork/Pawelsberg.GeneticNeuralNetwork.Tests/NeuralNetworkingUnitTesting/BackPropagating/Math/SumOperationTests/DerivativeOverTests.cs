using System.Collections.Generic;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.NeuralNetworkingUnitTesting.BackPropagating.Math.SumOperationTests;

public class DerivativeOverTests
{
    [Fact]
    public void DerivativeSumsChildDerivatives()
    {
        var synapse = new Synapse();
        var input = new Input { InputSynapse = synapse, Value = 2 };
        var multiplier = new Multiplier { Synapse = synapse, Value = 3 };

        var sum = new SumOperation
        {
            Expressions = new List<Expression>
            {
                multiplier,
                input
            }
        };

        var derivative = sum.DerivativeOver(multiplier);
        derivative.CalcValue();

        Assert.Equal(1d, derivative.Value); // derivative(multiplier)=1, derivative(input)=0
    }

    [Fact]
    public void DerivativeWithNoTargetIsZero()
    {
        var sum = new SumOperation
        {
            Expressions = new List<Expression>
            {
                new Constant { Value = 1 },
                new Input { InputSynapse = new Synapse(), Value = 2 }
            }
        };

        var derivative = sum.DerivativeOver(new Multiplier { Synapse = new Synapse(), Value = 1 });
        derivative.CalcValue();

        Assert.Equal(0d, derivative.Value);
    }

    [Fact]
    public void DerivativeHandlesMultipleTargets()
    {
        var synapse = new Synapse();
        var multiplier = new Multiplier { Synapse = synapse, Value = 3 };

        var sum = new SumOperation
        {
            Expressions = new List<Expression>
            {
                multiplier,
                new MultiplyOperation
                {
                    Expressions = new List<Expression>
                    {
                        new Constant { Value = 2 },
                        multiplier
                    }
                }
            }
        };

        var derivative = sum.DerivativeOver(multiplier);
        derivative.CalcValue();

        Assert.Equal(3d, derivative.Value); // 1 + 2
    }

    [Fact]
    public void DerivativeWithOnlyConstantsIsZero()
    {
        var sum = new SumOperation
        {
            Expressions = new List<Expression>
            {
                new Constant { Value = 2 },
                new Constant { Value = 3 }
            }
        };

        var derivative = sum.DerivativeOver(new Multiplier { Synapse = new Synapse(), Value = 1 });
        derivative.CalcValue();

        Assert.Equal(0d, derivative.Value);
    }

    [Fact]
    public void DerivativePreservesStructureCount()
    {
        var synapse = new Synapse();
        var multiplier = new Multiplier { Synapse = synapse, Value = 1 };
        var otherMultiplier = new Multiplier { Synapse = new Synapse(), Value = 1 };

        var sum = new SumOperation
        {
            Expressions = new List<Expression>
            {
                multiplier,
                otherMultiplier
            }
        };

        var derivative = sum.DerivativeOver(multiplier);

        var constant = Assert.IsType<Constant>(derivative);
        Assert.Equal(1d, constant.Value);
    }

    [Fact]
    public void DerivativeWithNestedSumsAddsAll()
    {
        var synapse = new Synapse();
        var multiplier = new Multiplier { Synapse = synapse, Value = 1 };
        var nested = new SumOperation
        {
            Expressions = new List<Expression>
            {
                multiplier,
                new Constant { Value = 2 }
            }
        };

        var sum = new SumOperation { Expressions = new List<Expression> { nested, new Constant { Value = 1 } } };

        var derivative = sum.DerivativeOver(multiplier);
        derivative.CalcValue();

        Assert.Equal(1d, derivative.Value);
    }
}
