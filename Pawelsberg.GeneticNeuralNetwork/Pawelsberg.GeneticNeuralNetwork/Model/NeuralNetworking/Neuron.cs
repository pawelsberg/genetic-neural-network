using System;
using System.Collections.Generic;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking
{
    public class Neuron : Node
    {
        public List<double> InputMultiplier { get; private set; }
        public List<Synapse> Inputs { get; private set; }
        public ActivationFunction ActivationFunction { get; set; }

        public Neuron()
        {
            Inputs = new List<Synapse>();
            InputMultiplier = new List<double>();
        }
        public void AddInput(Synapse synapse, double multiplier)
        {
            Inputs.Add(synapse);
            InputMultiplier.Add(multiplier);
        }
        public void RemoveInput(Synapse synapse)
        {
            int synapseIndex = Inputs.IndexOf(synapse);
            Inputs.RemoveAt(synapseIndex);
            InputMultiplier.RemoveAt(synapseIndex);
        }
    }
}
