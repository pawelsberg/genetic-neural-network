using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.Mutating
{
    public class SynapseReconnectorNetworkMutator : Mutator<Network>
    {
        public override MutationDescription Mutate(Network network)
        {
            TryReconnectRandomSynapse(network); // try reconnect random synapse from neuron - if it has synapses
            return new MutationDescription() { Text = "SynapseReconnector" };
        }

        private void TryReconnectRandomSynapse(Network network)
        {
            int nodeIndex = RandomGenerator.Random.Next(network.Nodes.Count);
            int newNodeIndex = RandomGenerator.Random.Next(network.Nodes.Count);

            Node node = network.Nodes[nodeIndex];
            Node newNode = network.Nodes[newNodeIndex];


            if (node != newNode)
            {
                if (RandomGenerator.Random.NextDouble() > 0.5d && node is Neuron && newNode is Neuron && (node as Neuron).Inputs.Count > 0)
                {
                    Neuron neuron = node as Neuron;
                    Neuron newNeuron = newNode as Neuron;

                    // take input
                    int inputSynapseIndex = RandomGenerator.Random.Next(neuron.Inputs.Count);
                    Synapse inputSynapse = neuron.Inputs[inputSynapseIndex];

                    // reconnect
                    newNeuron.AddInput(inputSynapse, neuron.InputMultiplier[inputSynapseIndex]);
                    neuron.RemoveInput(inputSynapse);
                }
                else if (node.Outputs.Count > 0)
                {
                    // take output
                    int outputSynapseIndex = RandomGenerator.Random.Next(node.Outputs.Count);
                    Synapse outputSynapse = node.Outputs[outputSynapseIndex];

                    // reconnect
                    newNode.AddOutput(outputSynapse);
                    node.RemoveOutput(outputSynapse);
                }
            }
        }
    }
}