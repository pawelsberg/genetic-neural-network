using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.NeuralNetworkingUnitTesting.BackPropagating.Math.ConstantTests;

public class DeepCloneTests
{
    [Fact]
    public void DeepCloneCopiesValue()
    {
        var constant = new Constant { Value = 7.5 };

        var clone = constant.DeepClone();

        Assert.IsType<Constant>(clone);
        Assert.NotSame(constant, clone);
        Assert.Equal(constant.Value, ((Constant)clone).Value);
    }
}
