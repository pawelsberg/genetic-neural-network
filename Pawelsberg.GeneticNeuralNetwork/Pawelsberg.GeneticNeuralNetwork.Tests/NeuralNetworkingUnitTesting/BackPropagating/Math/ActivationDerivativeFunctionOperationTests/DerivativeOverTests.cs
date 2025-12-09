using System;
using System.Collections.Generic;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.NeuralNetworkingUnitTesting.BackPropagating.Math.ActivationDerivativeFunctionOperationTests;

public class DerivativeOverTests
{
    [Fact]
    public void DerivativeThrowsForSecondDerivative()
    {
        var op = new ActivationDerivativeFunctionOperation
        {
            ActivationFunction = ActivationFunction.Sigmoid,
            Expression = new Constant { Value = 1 }
        };

        Assert.Throws<Exception>(() => op.DerivativeOver(new Multiplier { Synapse = new Synapse(), Value = 1 }));
    }
}
