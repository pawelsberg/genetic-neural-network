using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.NeuralNetworkingUnitTesting.BackPropagating.Math.ActivationDerivativeFunctionOperationTests;

public class DeepCloneTests
{
    [Fact]
    public void DeepCloneCopiesInner()
    {
        var inner = new Constant { Value = 2 };
        var op = new ActivationDerivativeFunctionOperation
        {
            ActivationFunction = ActivationFunction.Tanh,
            Expression = inner
        };

        var clone = op.DeepClone();

        var cloned = Assert.IsType<ActivationDerivativeFunctionOperation>(clone);
        Assert.NotSame(op, cloned);
        Assert.NotSame(inner, cloned.Expression);
        Assert.Equal(inner.Value, ((Constant)cloned.Expression).Value);
    }
}
