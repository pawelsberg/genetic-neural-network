using System.Collections.Generic;
using System.Linq;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting.DiskStoring;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.NeuralNetworkingUnitTesting.BackPropagating.NetworkExtensionTests;

public class BackPropagationTests
{
    [Fact]
    public void BackPropagationImprovesSimpleLinearNetwork()
    {
        var network = CreateNetwork(1d, 1.1d);
        var testCaseList = TestCaseList.GetSimpleSumTestCaseList();

        double errorBefore = TotalAbsoluteError(network, testCaseList);

        network.BackPropagation(testCaseList, propagations: 1);

        double errorAfterFirst = TotalAbsoluteError(network, testCaseList);

        network.BackPropagation(testCaseList, propagations: 1);

        double errorAfterSecond = TotalAbsoluteError(network, testCaseList);

        Assert.True(errorAfterFirst < errorBefore, $"Expected first step to decrease error. Before={errorBefore}, AfterFirst={errorAfterFirst}");
        Assert.True(errorAfterSecond < errorAfterFirst, $"Expected second step to further decrease error. AfterFirst={errorAfterFirst}, AfterSecond={errorAfterSecond}");
    }

    [Fact]
    public void BackPropagationImprovesWhenSecondWeightIsPointNine()
    {
        var network = CreateNetwork(1d, 0.9d);
        var testCaseList = TestCaseList.GetSimpleSumTestCaseList();

        double errorBefore = TotalAbsoluteError(network, testCaseList);

        network.BackPropagation(testCaseList, propagations: 1);

        double errorAfter = TotalAbsoluteError(network, testCaseList);

        Assert.True(errorAfter < errorBefore, $"Expected error to decrease. Before={errorBefore}, After={errorAfter}");
    }

    [Fact]
    public void BackPropagationImprovesWhenSecondWeightIsPointNineNine()
    {
        var network = CreateNetwork(1d, 0.99d);
        var testCaseList = TestCaseList.GetSimpleSumTestCaseList();

        double errorBefore = TotalAbsoluteError(network, testCaseList);

        network.BackPropagation(testCaseList, propagations: 1);

        double errorAfter = TotalAbsoluteError(network, testCaseList);

        Assert.True(errorAfter < errorBefore, $"Expected error to decrease. Before={errorBefore}, After={errorAfter}");
    }

    [Fact]
    public void BackPropagationImprovesWhenSecondWeightIsPointNineNineNineNine()
    {
        var network = CreateNetwork(1d, 0.9999d);
        var testCaseList = TestCaseList.GetSimpleSumTestCaseList();

        double errorBefore = TotalAbsoluteError(network, testCaseList);

        network.BackPropagation(testCaseList, propagations: 1);

        double errorAfter = TotalAbsoluteError(network, testCaseList);

        Assert.True(errorAfter < errorBefore, $"Expected error to decrease in one step. Before={errorBefore}, After={errorAfter}");
    }

    [Fact]
    public void BackPropagationImprovesWhenSecondWeightIsSlightlyAboveOne()
    {
        var network = CreateNetwork(1d, 1.0000001d);
        var testCaseList = TestCaseList.GetSimpleSumTestCaseList();

        double errorBefore = TotalAbsoluteError(network, testCaseList);

        network.BackPropagation(testCaseList, propagations: 1);

        double errorAfter = TotalAbsoluteError(network, testCaseList);

        Assert.True(errorAfter < errorBefore, $"Expected error to decrease in one step. Before={errorBefore}, After={errorAfter}");
    }

    [Fact]
    public void BackPropagationImprovesThreeLevelLinearNetwork()
    {
        var network = CreateThreeLevelLinearNetwork();
        var testCaseList = new TestCaseList
        {
            TestCases = new List<TestCase>
            {
                new TestCase { Inputs = new List<int> { 1 }, Outputs = new List<int> { 1 } },
                new TestCase { Inputs = new List<int> { 2 }, Outputs = new List<int> { 2 } },
                new TestCase { Inputs = new List<int> { -3 }, Outputs = new List<int> { -3 } }
            }
        };

        double errorBefore = TotalAbsoluteError(network, testCaseList);

        network.BackPropagation(testCaseList, propagations: 1);

        double errorAfter = TotalAbsoluteError(network, testCaseList);

        Assert.True(errorAfter < errorBefore, $"Expected error to decrease in one step for three-level network. Before={errorBefore}, After={errorAfter}");
    }

    [Fact]
    public void BackPropagationImprovesBranchingLinearNetwork()
    {
        var network = CreateBranchingLinearNetwork();
        var testCaseList = TestCaseList.GetSimpleSumTestCaseList();

        double errorBefore = TotalAbsoluteError(network, testCaseList);

        network.BackPropagation(testCaseList, propagations: 1);

        double errorAfter = TotalAbsoluteError(network, testCaseList);

        Assert.True(errorAfter < errorBefore, $"Expected error to decrease in one step for branching network. Before={errorBefore}, After={errorAfter}");
    }

    private static Network CreateNetwork(double multiplier0, double multiplier1)
    {
        var input0 = new Synapse();
        var input1 = new Synapse();
        var output0 = new Synapse();

        var neuron = new Neuron { ActivationFunction = ActivationFunction.Linear };
        neuron.AddInput(input0, multiplier0);
        neuron.AddInput(input1, multiplier1);
        neuron.AddOutput(output0);

        var network = new Network();
        network.Inputs.AddRange(new[] { input0, input1 });
        network.Outputs.Add(output0);
        network.Nodes.Add(neuron);

        return network;
    }

    private static Network CreateThreeLevelLinearNetwork()
    {
        var input = new Synapse();
        var hiddenOutput1 = new Synapse();
        var hiddenOutput2 = new Synapse();
        var output = new Synapse();

        var layer1 = new Neuron { ActivationFunction = ActivationFunction.Linear };
        layer1.AddInput(input, 0.9d);
        layer1.AddOutput(hiddenOutput1);

        var layer2 = new Neuron { ActivationFunction = ActivationFunction.Linear };
        layer2.AddInput(hiddenOutput1, 1.05000001d);
        layer2.AddOutput(hiddenOutput2);

        var layer3 = new Neuron { ActivationFunction = ActivationFunction.Linear };
        layer3.AddInput(hiddenOutput2, 1.01d);
        layer3.AddOutput(output);

        var network = new Network();
        network.Inputs.Add(input);
        network.Outputs.Add(output);
        network.Nodes.AddRange(new Node[] { layer1, layer2, layer3 });

        return network;
    }

    private static Network CreateBranchingLinearNetwork()
    {
        var input0 = new Synapse();
        var input1 = new Synapse();
        var hiddenOutput0 = new Synapse();
        var hiddenOutput1 = new Synapse();
        var output = new Synapse();

        var hidden0 = new Neuron { ActivationFunction = ActivationFunction.Linear };
        hidden0.AddInput(input0, 0.8d);
        hidden0.AddOutput(hiddenOutput0);

        var hidden1 = new Neuron { ActivationFunction = ActivationFunction.Linear };
        hidden1.AddInput(input1, 1.2d);
        hidden1.AddOutput(hiddenOutput1);

        var outputNeuron = new Neuron { ActivationFunction = ActivationFunction.Linear };
        outputNeuron.AddInput(hiddenOutput0, 0.9d);
        outputNeuron.AddInput(hiddenOutput1, 1.05d);
        outputNeuron.AddOutput(output);

        var network = new Network();
        network.Inputs.AddRange(new[] { input0, input1 });
        network.Outputs.Add(output);
        network.Nodes.AddRange(new Node[] { hidden0, hidden1, outputNeuron });

        return network;
    }

    private static double TotalAbsoluteError(Network network, TestCaseList testCaseList)
    {
        double totalError = 0d;

        foreach (var testCase in testCaseList.TestCases)
        {
            var runningContext = network.SafeRun(1);

            for (int index = 0; index < testCase.Inputs.Count; index++)
            {
                var inputSynapse = network.Inputs[index];
                int inputVal = testCase.Inputs[index];
                var inputs = runningContext.SynapseExpressions
                    .Values
                    .SelectMany(expr => expr.GetExpressionsRecursive())
                    .OfType<Input>()
                    .Where(input => input.InputSynapse == inputSynapse);

                foreach (var input in inputs)
                    input.Value = inputVal;
            }

            foreach (var outputSynapse in network.Outputs)
            {
                var expression = runningContext.SynapseExpressions[outputSynapse];
                expression.CalcValue();
                int expected = testCase.Outputs[network.Outputs.IndexOf(outputSynapse)];
                totalError += System.Math.Abs(expected - expression.Value);
            }
        }

        return totalError;
    }
}
