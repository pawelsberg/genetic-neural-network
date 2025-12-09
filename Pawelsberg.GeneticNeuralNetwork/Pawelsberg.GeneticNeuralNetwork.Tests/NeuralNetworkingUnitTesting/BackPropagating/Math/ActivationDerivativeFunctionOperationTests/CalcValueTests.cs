using System;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.NeuralNetworkingUnitTesting.BackPropagating.Math.ActivationDerivativeFunctionOperationTests;

public class CalcValueTests
{
    [Fact]
    public void CalcValueAppliesActivationDerivative()
    {
        var inner = new Constant { Value = 0 };
        var op = new ActivationDerivativeFunctionOperation
        {
            ActivationFunction = ActivationFunction.Sigmoid,
            Expression = inner
        };

        op.CalcValue();

        Assert.Equal(0.25d, op.Value, 6); // sigmoid'(0)=0.25
    }

    [Theory]
    [InlineData(-10d)]
    [InlineData(0d)]
    [InlineData(10d)]
    public void CalcValueLinearIsAlwaysOne(double input)
    {
        var op = new ActivationDerivativeFunctionOperation
        {
            ActivationFunction = ActivationFunction.Linear,
            Expression = new Constant { Value = input }
        };

        op.CalcValue();

        Assert.Equal(1d, op.Value, 6);
    }

    [Theory]
    [InlineData(-2d, 0d)]
    [InlineData(-1d, 1d)]
    [InlineData(0d, 1d)]
    [InlineData(1d, 1d)]
    [InlineData(2d, 0d)]
    public void CalcValueThresholdMatchesRange(double input, double expected)
    {
        var op = new ActivationDerivativeFunctionOperation
        {
            ActivationFunction = ActivationFunction.Threshold,
            Expression = new Constant { Value = input }
        };

        op.CalcValue();

        Assert.Equal(expected, op.Value, 6);
    }

    [Theory]
    [InlineData(-1d, 0d)]
    [InlineData(0d, 1d)]
    [InlineData(0.5d, 1d)]
    [InlineData(1d, 1d)]
    [InlineData(2d, 0d)]
    public void CalcValueSquashingMatchesRange(double input, double expected)
    {
        var op = new ActivationDerivativeFunctionOperation
        {
            ActivationFunction = ActivationFunction.Squashing,
            Expression = new Constant { Value = input }
        };

        op.CalcValue();

        Assert.Equal(expected, op.Value, 6);
    }

    [Theory]
    [InlineData(-50d, 0d)]
    [InlineData(0d, 0.25d)]
    [InlineData(50d, 0d)]
    public void CalcValueSigmoidDerivativeExtremes(double input, double expected)
    {
        var op = new ActivationDerivativeFunctionOperation
        {
            ActivationFunction = ActivationFunction.Sigmoid,
            Expression = new Constant { Value = input }
        };

        op.CalcValue();

        const double epsilon = 1e-6;
        Assert.True(System.Math.Abs(op.Value - expected) < epsilon, $"Expected ~{expected} within {epsilon}, got {op.Value}");
    }

    [Theory]
    [InlineData(-20d, 0d)]
    [InlineData(0d, 1d)]
    [InlineData(1d, 0.41997434161402614d)]
    [InlineData(20d, 0d)]
    public void CalcValueTanhDerivativeMatchesFormula(double input, double expected)
    {
        var op = new ActivationDerivativeFunctionOperation
        {
            ActivationFunction = ActivationFunction.Tanh,
            Expression = new Constant { Value = input }
        };

        op.CalcValue();

        const double epsilon = 1e-6;
        Assert.True(System.Math.Abs(op.Value - expected) < epsilon, $"Expected ~{expected} within {epsilon}, got {op.Value}");
    }
}
