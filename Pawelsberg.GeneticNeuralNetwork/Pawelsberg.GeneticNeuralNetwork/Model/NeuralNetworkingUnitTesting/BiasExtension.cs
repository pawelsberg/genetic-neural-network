using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting
{
    public static class BiasExtension
    {
        public static void Propagate(this Bias thisBias, RunningContext runningContext)
        {
            double outputPotential = 1d;

            foreach (Synapse outputSynapse in thisBias.Outputs)
            {
                // copy potential to output synapse
                runningContext.SetPotential(outputSynapse, outputPotential);
            }
        }
    }
}