using System.Collections.Generic;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.Mutating
{
    public class SynapseRemoverNetworkMutator : Mutator<Network>
    {
        public override MutationDescription Mutate(Network network)
        {
            int nodeIndex = RandomGenerator.Random.Next(network.Nodes.Count);

            List<Synapse> nodeSynapses = new List<Synapse>();

            if (network.Nodes[nodeIndex] is Neuron)
                nodeSynapses.AddRange((network.Nodes[nodeIndex] as Neuron).Inputs);
            nodeSynapses.AddRange(network.Nodes[nodeIndex].Outputs);

            // try to remove synapse - if randomly picket is not input or output and if randomly picked neuron has synapses
            if (nodeSynapses.Count > 0)
            {
                int synapseIndex = RandomGenerator.Random.Next(nodeSynapses.Count);
                if (!network.Inputs.Contains(nodeSynapses[synapseIndex]) && !network.Outputs.Contains(nodeSynapses[synapseIndex]))
                    network.RemoveSynapse(nodeSynapses[synapseIndex]);
            }
            return new MutationDescription() { Text = "SynapseRemover" };
        }
    }
}