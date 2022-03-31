using System.Collections.Generic;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting
{
    public static class NeuronExtension
    {
        public static void Propagate(this Neuron thisNeuron, RunningContext runningContext)
        {
            double outputPotential = 0;

            // calculate output potential
            Dictionary<Synapse, double> synapsePotentials = runningContext.SynapsePotentials;
            for (int inputIndex = 0; inputIndex < thisNeuron.Inputs.Count; inputIndex++)
            {
                Synapse inputSynapse = thisNeuron.Inputs[inputIndex];
                outputPotential
                    +=
                    synapsePotentials[inputSynapse]
                    *
                    thisNeuron.InputMultiplier[inputIndex];
            }

            outputPotential = thisNeuron.ActivationFunction.Apply(outputPotential);

            foreach (Synapse outputSynapse in thisNeuron.Outputs)
            {
                // copy potential to output synapse
                runningContext.SetPotential(outputSynapse, outputPotential);
            }
        }
    }
}
