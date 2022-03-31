using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.Mutating
{
    public class SynapseReducerNetworkMutator : Mutator<Network>
    {
        public override MutationDescription Mutate(Network network)
        {
            foreach (Node sourceNode in network.Nodes)
            {
                List<Synapse> synapsesToRemove = new List<Synapse>();
                foreach (Synapse synapse in sourceNode.Outputs)
                {
                    Neuron destinationNeuron = network.Nodes.OfType<Neuron>().FirstOrDefault(nrn => nrn.Inputs.Contains(synapse));

                    // if found second synapse with the same source and destination then merge
                    if (destinationNeuron != null)
                    {
                        Synapse notRemovedOtherSynapse = sourceNode.Outputs.FirstOrDefault(snps => !synapsesToRemove.Contains(snps) && snps != synapse && destinationNeuron.Inputs.Contains(snps));

                        if (notRemovedOtherSynapse != null)
                        {
                            int notRemovedOtherSynapseIndex = destinationNeuron.Inputs.IndexOf(notRemovedOtherSynapse);
                            int synapseIndex = destinationNeuron.Inputs.IndexOf(synapse);

                            destinationNeuron.InputMultiplier[notRemovedOtherSynapseIndex] += destinationNeuron.InputMultiplier[synapseIndex];
                            destinationNeuron.RemoveInput(synapse);
                            synapsesToRemove.Add(synapse);
                        }
                    }
                }

                foreach (Synapse synapse in synapsesToRemove)
                {
                    sourceNode.RemoveOutput(synapse);
                }
            }
            return new MutationDescription() { Text = "SynapseReducer" };
        }
    }
}