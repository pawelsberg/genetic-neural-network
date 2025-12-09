using System.Collections.Generic;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.NeuralNetworkingUnitTesting.BackPropagating.Math.SumOperationTests;

public class ToStringTests
{
    [Fact]
    public void ToStringJoinsTermsWithPlus()
    {
        var expr = new SumOperation
        {
            Expressions = new List<Expression>
            {
                new Constant { Value = 2 },
                new Constant { Value = 3 }
            }
        };

        Assert.Equal("((C:2)+(C:3))", expr.ToString());
    }
}
