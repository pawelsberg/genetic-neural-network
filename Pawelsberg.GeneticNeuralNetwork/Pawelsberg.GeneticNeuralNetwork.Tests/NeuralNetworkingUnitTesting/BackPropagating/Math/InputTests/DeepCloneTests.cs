using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.NeuralNetworkingUnitTesting.BackPropagating.Math.InputTests;

public class DeepCloneTests
{
    [Fact]
    public void DeepCloneCopiesSynapseAndValue()
    {
        var synapse = new Synapse();
        var input = new Input { InputSynapse = synapse, Value = 4 };

        var clone = input.DeepClone();

        var clonedInput = Assert.IsType<Input>(clone);
        Assert.NotSame(input, clonedInput);
        Assert.Same(synapse, clonedInput.InputSynapse);
        Assert.Equal(input.Value, clonedInput.Value);
    }
}
