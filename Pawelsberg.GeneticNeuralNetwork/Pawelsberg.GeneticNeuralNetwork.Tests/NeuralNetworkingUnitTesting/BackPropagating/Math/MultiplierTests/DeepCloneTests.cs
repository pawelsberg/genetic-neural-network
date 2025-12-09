using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.NeuralNetworkingUnitTesting.BackPropagating.Math.MultiplierTests;

public class DeepCloneTests
{
    [Fact]
    public void DeepCloneCopiesSynapseAndValue()
    {
        var synapse = new Synapse();
        var multiplier = new Multiplier { Synapse = synapse, Value = 4.2 };

        var clone = multiplier.DeepClone();

        var clonedMultiplier = Assert.IsType<Multiplier>(clone);
        Assert.NotSame(multiplier, clonedMultiplier);
        Assert.Same(synapse, clonedMultiplier.Synapse);
        Assert.Equal(multiplier.Value, clonedMultiplier.Value);
    }
}
