using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.Mutating;

public class TransparentNeuronAdderNetworkMutator : Mutator<Network>
{
    public int MaxNodes { get; set; }
    public int MaxSynapses { get; set; }

    public TransparentNeuronAdderNetworkMutator(int maxNodes, int maxSynapses)
    {
        MaxNodes = maxNodes;
        MaxSynapses = maxSynapses;
    }

    public override MutationDescription Mutate(Network network)
    {
        TryInsertRandomTransparentNeuron(network);
        return new MutationDescription() { Text = "TransparentNeuronAdder" };
    }

    private void TryInsertRandomTransparentNeuron(Network network)
    {
        List<Synapse> allSynapses = new List<Synapse>(network.GetAllSynapses());
        if (network.Nodes.Count < MaxNodes && allSynapses.Count < MaxSynapses)
        {
            int synapseIndex = RandomGenerator.Random.Next(allSynapses.Count - 1);
            Synapse synapse = allSynapses[synapseIndex];

            if (!network.Inputs.Contains(synapse))
            { // currently just for inner synapses
                Node inputNode = network.Nodes.Single(nrn => nrn.Outputs.Contains(synapse));
                inputNode.RemoveOutput(synapse);
                Synapse newSynapse = new Synapse();
                inputNode.AddOutput(newSynapse);
                Neuron newNeuron = new Neuron();
                newNeuron.AddInput(newSynapse, 1d);
                newNeuron.AddOutput(synapse);
                network.Nodes.Add(newNeuron);
            }
            else
            {
                Neuron outputNeuron = network.Nodes.OfType<Neuron>().Single(nrn => nrn.Inputs.Contains(synapse));
                int synapseInputIndex = outputNeuron.Inputs.IndexOf(synapse);
                double multiplier = outputNeuron.InputMultiplier[synapseInputIndex];
                outputNeuron.RemoveInput(synapse);
                Synapse newSynapse = new Synapse();
                outputNeuron.AddInput(newSynapse, multiplier);
                Neuron newNeuron = new Neuron();
                newNeuron.AddInput(synapse, 1d);
                newNeuron.AddOutput(newSynapse);
                network.Nodes.Add(newNeuron);
            }
        }
    }
}
