using System.Linq;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.Mutating
{
    public class NeuronReducerNetworkMutator : Mutator<Network>
    {
        public override MutationDescription Mutate(Network network)
        {
            while (true)
            {
                Neuron neuron = network.Nodes.OfType<Neuron>().FirstOrDefault(
                    nrn => nrn.Inputs.Count == 1
                        && nrn.Outputs.Count == 1
                        && !network.Inputs.Contains(nrn.Inputs[0])
                        && !network.Outputs.Contains(nrn.Outputs[0])
                        && nrn.Inputs[0] != nrn.Outputs[0]);

                if (neuron != null)
                {
                    double multiplier = neuron.InputMultiplier[0];
                    Synapse inputSynapse = neuron.Inputs[0];
                    Synapse outputSynapse = neuron.Outputs[0];
                    Neuron outputNeuron = network.Nodes.OfType<Neuron>().Single(nrn => nrn.Inputs.Contains(outputSynapse));
                    int outputNeuronSynapseIndex = outputNeuron.Inputs.IndexOf(outputSynapse);
                    outputNeuron.Inputs[outputNeuronSynapseIndex] = inputSynapse;
                    outputNeuron.InputMultiplier[outputNeuronSynapseIndex] *= multiplier;

                    network.Nodes.Remove(neuron);
                }
                else break;
            }
            return new MutationDescription() { Text = "NeuronReducer" };

        }
    }
}