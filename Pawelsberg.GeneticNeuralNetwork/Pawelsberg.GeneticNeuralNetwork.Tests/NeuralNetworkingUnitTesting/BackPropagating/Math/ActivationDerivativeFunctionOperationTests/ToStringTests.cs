using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.NeuralNetworkingUnitTesting.BackPropagating.Math.ActivationDerivativeFunctionOperationTests;

public class ToStringTests
{
    [Fact]
    public void ToStringUsesLetterCodePrime()
    {
        var op = new ActivationDerivativeFunctionOperation
        {
            ActivationFunction = ActivationFunction.Sigmoid,
            Expression = new Constant { Value = 1 }
        };

        Assert.Equal("Σ'((C:1))", op.ToString());
    }
}
