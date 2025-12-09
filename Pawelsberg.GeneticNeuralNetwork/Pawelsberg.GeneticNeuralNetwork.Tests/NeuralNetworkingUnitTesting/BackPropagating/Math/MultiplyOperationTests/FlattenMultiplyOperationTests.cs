using System.Collections.Generic;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.NeuralNetworkingUnitTesting.BackPropagating.Math.MultiplyOperationTests;

public class FlattenMultiplyOperationTests
{
    [Fact]
    public void FlattenReturnsAllNestedFactors()
    {
        var nested = new MultiplyOperation
        {
            Expressions = new List<Expression>
            {
                new Constant { Value = 2 },
                new MultiplyOperation
                {
                    Expressions = new List<Expression>
                    {
                        new Constant { Value = 3 },
                        new Constant { Value = 5 }
                    }
                }
            }
        };

        var factors = nested.FlattenMultiplyOperation();

        Assert.Equal(3, factors.Count);
    }
}
