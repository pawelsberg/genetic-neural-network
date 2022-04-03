using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting.DiskStoring;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting;

public static class TestCaseListNetworkExtension
{
    public static Network CreateNetwork(this TestCaseList thisTestCaseList)
    {
        Network network = new Network();

        Bias bias = new Bias();
        network.Nodes.Add(bias);

        int inputsCount = thisTestCaseList.TestCases.Max(tc => tc.Inputs.Count);
        int outputCount = thisTestCaseList.TestCases.Max(tc => tc.Outputs.Count);

        network.Inputs.AddRange(Enumerable.Range(0, inputsCount).Select(i => new Synapse()));
        network.Outputs.AddRange(Enumerable.Range(0, outputCount).Select(i => new Synapse()));

        List<Neuron> inputNeurons = network.Inputs.Select(inputSynapse =>
            new Neuron()
            {
                Inputs = { inputSynapse },
                ActivationFunction = ActivationFunction.Linear,
                InputMultiplier = { 1 }
            }).ToList();
        network.Nodes.AddRange(inputNeurons);

        List<Neuron> outputNeurons = network.Outputs.Select(outputSynapse =>
            new Neuron()
            {
                Outputs = { outputSynapse },
                ActivationFunction = ActivationFunction.Linear

            }).ToList();

        foreach (TestCase testCase in thisTestCaseList.TestCases)
        {
            Neuron testCaseCountNeuron = new Neuron(); // will get as a value number of inputs satisfying test case

            for (int tcInputIndex = 0; tcInputIndex < testCase.Inputs.Count; tcInputIndex++)
            {
                Neuron inputNeuron = inputNeurons[tcInputIndex];
                int tcInput = testCase.Inputs[tcInputIndex];


                Synapse valueCheckSynapse = network.CreateJeNetworkPart(inputNeuron, tcInput, bias);
                testCaseCountNeuron.AddInput(valueCheckSynapse, 1);

            }
            network.Nodes.Add(testCaseCountNeuron);
            Synapse testCaseSynapse = network.CreateJeNetworkPart(testCaseCountNeuron, testCase.Inputs.Count, bias);

            Neuron testCaseNeuron = new Neuron(); // 1 if test case detected, 0 otherwise
            network.Nodes.Add(testCaseNeuron);

            testCaseNeuron.AddInput(testCaseSynapse, 1);


            for (int tcOutputIndex = 0; tcOutputIndex < testCase.Outputs.Count; tcOutputIndex++)
            {
                Neuron outputNeuron = outputNeurons[tcOutputIndex];
                int tcOutput = testCase.Outputs[tcOutputIndex];

                if (tcOutput != 0)
                {
                    Synapse testCaseOutputSynapse = new Synapse();
                    outputNeuron.AddInput(testCaseOutputSynapse, tcOutput);
                    testCaseNeuron.AddOutput(testCaseOutputSynapse);
                }
            }
        }
        network.Nodes.AddRange(outputNeurons);
        return network;
    }

}
