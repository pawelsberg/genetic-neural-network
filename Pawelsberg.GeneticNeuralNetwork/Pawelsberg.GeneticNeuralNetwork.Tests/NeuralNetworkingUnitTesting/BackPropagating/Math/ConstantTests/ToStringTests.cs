using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.NeuralNetworkingUnitTesting.BackPropagating.Math.ConstantTests;

public class ToStringTests
{
    [Fact]
    public void ToStringFormatsWithValue()
    {
        var constant = new Constant { Value = 3.14 };

        Assert.Equal("(C:3.14)", constant.ToString());
    }
}
