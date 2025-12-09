using System.Collections.Generic;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.NeuralNetworkingUnitTesting.BackPropagating.Math.MultiplyOperationTests;

public class DeepCloneTests
{
    [Fact]
    public void DeepCloneCopiesStructure()
    {
        var expr = new MultiplyOperation
        {
            Expressions = new List<Expression>
            {
                new Constant { Value = 2 },
                new Input { InputSynapse = new Synapse(), Value = 4 }
            }
        };

        var clone = expr.DeepClone();

        var cloned = Assert.IsType<MultiplyOperation>(clone);
        Assert.NotSame(expr, cloned);
        Assert.Equal(expr.Expressions.Count, cloned.Expressions.Count);
    }
}
