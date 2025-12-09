using System.Collections.Generic;
using System.Linq;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.NeuralNetworkingUnitTesting.BackPropagating.Math.ExpressionTests;

public class GetExpressionsRecursiveTests
{
    [Fact]
    public void ReturnsAllNestedExpressionsIncludingRoot()
    {
        var inner = new MultiplyOperation
        {
            Expressions = new List<Expression>
            {
                new Constant { Value = 2 },
                new Input { InputSynapse = new Synapse(), Value = 3 }
            }
        };

        var sum = new SumOperation { Expressions = new List<Expression> { inner, new Constant { Value = 1 } } };

        var all = sum.GetExpressionsRecursive().ToList();

        Assert.Contains(sum, all);
        Assert.Equal(5, all.Count);
    }
}
