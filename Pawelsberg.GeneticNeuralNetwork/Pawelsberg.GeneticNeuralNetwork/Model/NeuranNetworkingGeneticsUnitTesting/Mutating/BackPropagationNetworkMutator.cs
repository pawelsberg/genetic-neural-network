using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting.DiskStoring;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting.Mutating;

public class BackPropagationNetworkMutator : Mutator<Network>
{
    public TestCaseList TestCaseList { get; private set; }
    public int Propagations { get; private set; }
    public double Strength { get; private set; }

    public BackPropagationNetworkMutator(TestCaseList testCaseList, int propagations)
    {
        TestCaseList = testCaseList;
        Propagations = propagations;
        Strength = 0.1d;
    }

    public override MutationDescription Mutate(Network network)
    {
        if (network.Inputs.Count < TestCaseList.TestCases.Max(tc => tc.Inputs.Count)
            || network.Outputs.Count < TestCaseList.TestCases.Max(tc => tc.Outputs.Count))
            return new MutationDescription() { Text = "No Back Propagation" };

        network.BackPropagation(TestCaseList, Propagations);


        return new MutationDescription() { Text = "BackPropagation" };
    }
    private void OldBackProp(Network network)
    {
        double testCaseLists = TestCaseList.TestCases.Count;
        foreach (TestCase testCase in TestCaseList.TestCases)
        {
            if (network.Inputs.Count < testCase.Inputs.Count || network.Outputs.Count < testCase.Outputs.Count)
                break;

            NeuralNetworkingUnitTesting.RunningContext runningContext = network.SafeRun(testCase, Propagations);
            foreach (Synapse outputSynapse in network.Outputs)
            {
                Neuron outputNeuron = network.Nodes.Single(nd => nd.Outputs.Contains(outputSynapse)) as Neuron;

                if (outputNeuron == null)
                    break; // we cannot do anything for Bias nodes

                double actualOutput = runningContext.SynapsePotentials[outputSynapse];

                if (double.IsNaN(actualOutput))
                    break;

                int outputIndex = network.Outputs.IndexOf(outputSynapse);

                if (outputIndex >= testCase.Outputs.Count)
                    break; // there is no corresponding output in test case

                double expectedOutput = testCase.Outputs[outputIndex];
                double relativeError = expectedOutput / actualOutput;
                if (double.IsNaN(relativeError) || double.IsInfinity(relativeError))
                    break;

                double correction = Math.Max(-2, Math.Min(2, relativeError)) * Strength / testCaseLists / outputNeuron.Inputs.Count;

                for (int outNeuronInputIndex = 0; outNeuronInputIndex < outputNeuron.Inputs.Count; outNeuronInputIndex++)
                {
                    double inputMultiplier = outputNeuron.InputMultiplier[outNeuronInputIndex];
                    inputMultiplier *= 1 + correction;
                    outputNeuron.InputMultiplier[outNeuronInputIndex] = inputMultiplier;
                }
            }
        }

    }
}
