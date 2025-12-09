using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.NeuralNetworkingUnitTesting.BackPropagating.Math.ActivationFunctionOperationTests;

public class ToStringTests
{
    [Fact]
    public void ToStringUsesLetterCode()
    {
        var op = new ActivationFunctionOperation
        {
            ActivationFunction = ActivationFunction.Tanh,
            Expression = new Constant { Value = 1 }
        };

        Assert.Equal("H((C:1))", op.ToString());
    }
}
