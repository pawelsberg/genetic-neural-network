using System.Collections.Generic;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.NeuralNetworkingUnitTesting.BackPropagating.Math.SumOperationTests;

public class CalcValueTests
{
    [Fact]
    public void CalcValueSumsAllTerms()
    {
        var expr = new SumOperation
        {
            Expressions = new List<Expression>
            {
                new Constant { Value = 2 },
                new Constant { Value = 3 },
                new Input { InputSynapse = new Synapse(), Value = 4 }
            }
        };

        expr.CalcValue();

        Assert.Equal(9d, expr.Value);
    }
}
