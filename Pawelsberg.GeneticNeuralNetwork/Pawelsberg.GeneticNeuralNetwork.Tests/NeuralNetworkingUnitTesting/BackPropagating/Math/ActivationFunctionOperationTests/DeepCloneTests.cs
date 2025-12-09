using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.NeuralNetworkingUnitTesting.BackPropagating.Math.ActivationFunctionOperationTests;

public class DeepCloneTests
{
    [Fact]
    public void DeepCloneCopiesInner()
    {
        var inner = new Constant { Value = 3 };
        var op = new ActivationFunctionOperation
        {
            ActivationFunction = ActivationFunction.Linear,
            Expression = inner
        };

        var clone = op.DeepClone();

        var cloned = Assert.IsType<ActivationFunctionOperation>(clone);
        Assert.NotSame(op, cloned);
        Assert.NotSame(inner, cloned.Expression);
        Assert.Equal(inner.Value, ((Constant)cloned.Expression).Value);
    }
}
