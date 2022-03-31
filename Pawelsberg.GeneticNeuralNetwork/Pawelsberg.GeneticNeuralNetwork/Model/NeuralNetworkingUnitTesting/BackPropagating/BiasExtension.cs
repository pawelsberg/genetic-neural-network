using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating
{
    public static class BiasExtension
    {
        public static void Propagate(this Bias thisBias, RunningContext runningContext)
        {
            Constant outputExpression = new Constant { Value = 1 };

            foreach (Synapse outputSynapse in thisBias.Outputs)
            {
                // copy potential to output synapse
                runningContext.SetExpression(outputSynapse, outputExpression);
            }
        }
    }
}