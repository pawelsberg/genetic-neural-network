using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.Mutating
{
    public class NeuronMergerNetworkMutator : Mutator<Network>
    {
        public override MutationDescription Mutate(Network network)
        {
            TryReconnectRandomSynapse(network); // try reconnect random synapse from neuron - if it has synapses
            return new MutationDescription() { Text = "NeuronMerger" };
        }

        private void TryReconnectRandomSynapse(Network network)
        {
            int neuronIndex = RandomGenerator.Random.Next(network.Nodes.Count);
            int newNeuronIndex = RandomGenerator.Random.Next(network.Nodes.Count);

            if (network.Nodes[neuronIndex] is Neuron && network.Nodes[newNeuronIndex] is Neuron)
            {
                Neuron neuron = network.Nodes[neuronIndex] as Neuron;
                Neuron newNeuron = network.Nodes[newNeuronIndex] as Neuron;

                if (neuron != newNeuron)
                {
                    List<Synapse> inputs = new List<Synapse>(neuron.Inputs);
                    List<Synapse> outputs = new List<Synapse>(neuron.Outputs);
                    foreach (Synapse input in inputs)
                    {
                        double multiplier = neuron.InputMultiplier[neuron.Inputs.IndexOf(input)];
                        newNeuron.AddInput(input, multiplier);
                        neuron.RemoveInput(input);
                    }
                    foreach (Synapse output in outputs)
                    {
                        newNeuron.AddOutput(output);
                        neuron.RemoveOutput(output);
                    }
                    network.Nodes.Remove(neuron);
                }

            }
        }
    }
}