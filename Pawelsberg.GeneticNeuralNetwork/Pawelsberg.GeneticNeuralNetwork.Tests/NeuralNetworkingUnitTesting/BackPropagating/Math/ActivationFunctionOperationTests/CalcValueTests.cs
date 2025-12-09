using System;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.NeuralNetworkingUnitTesting.BackPropagating.Math.ActivationFunctionOperationTests;

public class CalcValueTests
{
    [Fact]
    public void CalcValueAppliesActivation()
    {
        var inner = new Constant { Value = 0 };
        var op = new ActivationFunctionOperation
        {
            ActivationFunction = ActivationFunction.Sigmoid,
            Expression = inner
        };

        op.CalcValue();

        Assert.Equal(0.5d, op.Value, 6);
    }

    [Theory]
    [InlineData(-3.5, -3.5)]
    [InlineData(7.2, 7.2)]
    public void CalcValueLinearPassesThrough(double input, double expected)
    {
        var op = new ActivationFunctionOperation { ActivationFunction = ActivationFunction.Linear, Expression = new Constant { Value = input } };

        op.CalcValue();

        Assert.Equal(expected, op.Value, 6);
    }

    [Theory]
    [InlineData(-5d, -1d)]
    [InlineData(-1d, -1d)]
    [InlineData(0.5d, 0.5d)]
    [InlineData(1d, 1d)]
    [InlineData(3d, 1d)]
    public void CalcValueThresholdClampsBetweenMinusOneAndOne(double input, double expected)
    {
        var op = new ActivationFunctionOperation { ActivationFunction = ActivationFunction.Threshold, Expression = new Constant { Value = input } };

        op.CalcValue();

        Assert.Equal(expected, op.Value, 6);
    }

    [Theory]
    [InlineData(-0.5d, 0d)]
    [InlineData(0d, 0d)]
    [InlineData(0.4d, 0.4d)]
    [InlineData(1d, 1d)]
    [InlineData(2d, 1d)]
    public void CalcValueSquashingClampsZeroToOne(double input, double expected)
    {
        var op = new ActivationFunctionOperation { ActivationFunction = ActivationFunction.Squashing, Expression = new Constant { Value = input } };

        op.CalcValue();

        Assert.Equal(expected, op.Value, 6);
    }

    [Theory]
    [InlineData(-50d, 0d)]
    [InlineData(0d, 0.5d)]
    [InlineData(50d, 1d)]
    public void CalcValueSigmoidApproachesLimits(double input, double expected)
    {
        var op = new ActivationFunctionOperation { ActivationFunction = ActivationFunction.Sigmoid, Expression = new Constant { Value = input } };

        op.CalcValue();

        const double epsilon = 1e-6;
        Assert.True(System.Math.Abs(op.Value - expected) < epsilon, $"Expected ~{expected} within {epsilon}, got {op.Value}");
    }

    [Theory]
    [InlineData(-20d, -1d)]
    [InlineData(-1d, -0.7615941559557649d)]
    [InlineData(0d, 0d)]
    [InlineData(1d, 0.7615941559557649d)]
    [InlineData(20d, 1d)]
    public void CalcValueTanhHandlesRange(double input, double expected)
    {
        var op = new ActivationFunctionOperation { ActivationFunction = ActivationFunction.Tanh, Expression = new Constant { Value = input } };

        op.CalcValue();

        const double epsilon = 1e-6;
        Assert.True(System.Math.Abs(op.Value - expected) < epsilon, $"Expected ~{expected} within {epsilon}, got {op.Value}");
    }
}
