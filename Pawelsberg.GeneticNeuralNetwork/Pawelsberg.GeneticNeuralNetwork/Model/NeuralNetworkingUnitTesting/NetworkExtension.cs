using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting
{
    public static class NetworkExtension
    {
        public static void SafeRun(this Network thisNetwork, RunningContext runningContext, int propagations)
        {
            for (int networkPropagation = 0; networkPropagation < propagations; networkPropagation++)
                foreach (Node node in thisNetwork.Nodes)
                {
                    // put it in NodeExtension?
                    if (node is Neuron)
                        ((Neuron)node).Propagate(runningContext);
                    else if (node is Bias)
                        ((Bias)node).Propagate(runningContext);
                }
        }

        public static RunningContext SafeRun(this Network thisNetwork, TestCase testCase, int propagations)
        {
            RunningContext runningContext = new RunningContext();
            // zero all potentials at the beginning
            foreach (Synapse synapse in thisNetwork.GetAllSynapses())
            {
                runningContext.SynapsePotentials.Add(synapse, 0d);
            }
            // set input potentials
            for (int inputIndex = 0; inputIndex < testCase.Inputs.Count; inputIndex++)
            {
                runningContext.SetPotential(thisNetwork.Inputs[inputIndex], testCase.Inputs[inputIndex]);
            }
            thisNetwork.SafeRun(runningContext, propagations);
            return runningContext;
        }

        public static Synapse CreateJeNetworkPart(this Network thisNetwork, Node inputNode, double value, Bias bias)
        {
            Synapse inputSynapse = new Synapse();
            inputNode.AddOutput(inputSynapse);

            Neuron zeroNeuron = new Neuron();
            thisNetwork.Nodes.Add(zeroNeuron);
            zeroNeuron.AddInput(inputSynapse, 1);

            if (value != 0)
            {
                Synapse biasSynapse = new Synapse();
                bias.AddOutput(biasSynapse);
                zeroNeuron.AddInput(biasSynapse, -value); // subtract the value so that the rest is "if zero"
            }

            Synapse oneInputSynapse = new Synapse();
            zeroNeuron.AddOutput(oneInputSynapse);

            Neuron oneNeuron = new Neuron { ActivationFunction = ActivationFunction.Squashing };
            thisNetwork.Nodes.Add(oneNeuron);
            oneNeuron.AddInput(oneInputSynapse, -1);

            Synapse twoInputSynapse = new Synapse();
            zeroNeuron.AddOutput(twoInputSynapse);

            Neuron twoNeuron = new Neuron { ActivationFunction = ActivationFunction.Squashing };
            thisNetwork.Nodes.Add(twoNeuron);
            twoNeuron.AddInput(twoInputSynapse, 1);

            Synapse oneOutputSynapse = new Synapse();
            oneNeuron.AddOutput(oneOutputSynapse);

            Synapse twoOutputSynapse = new Synapse();
            twoNeuron.AddOutput(twoOutputSynapse);

            Neuron outputNeuron = new Neuron();
            thisNetwork.Nodes.Add(outputNeuron);
            outputNeuron.AddInput(oneOutputSynapse, -1);
            outputNeuron.AddInput(twoOutputSynapse, -1);

            Synapse outputBiasSynapse = new Synapse();
            bias.AddOutput(outputBiasSynapse);
            outputNeuron.AddInput(outputBiasSynapse, 1);

            Synapse outputSynapse = new Synapse();
            outputNeuron.AddOutput(outputSynapse);

            return outputSynapse;
        }
    }
}
