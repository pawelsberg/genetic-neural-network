using System.Collections.Generic;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.NeuralNetworkingUnitTesting.BackPropagating.Math.SumOperationTests;

public class DeepCloneTests
{
    [Fact]
    public void DeepCloneCopiesStructure()
    {
        var expr = new SumOperation
        {
            Expressions = new List<Expression>
            {
                new Constant { Value = 1 },
                new Input { InputSynapse = new Synapse(), Value = 2 }
            }
        };

        var clone = expr.DeepClone();

        var cloned = Assert.IsType<SumOperation>(clone);
        Assert.NotSame(expr, cloned);
        Assert.Equal(expr.Expressions.Count, cloned.Expressions.Count);
    }
}
