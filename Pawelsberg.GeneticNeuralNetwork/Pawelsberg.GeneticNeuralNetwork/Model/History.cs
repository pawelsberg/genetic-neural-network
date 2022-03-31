using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;

namespace Pawelsberg.GeneticNeuralNetwork.Model
{
    // TODO - remove usage of static class
    public static class History
    {
        public static Network BestEver { get; set; }
        public static double BestEverQuality { get; set; }
        static History()
        {
            BestEverQuality = -1d;
        }
    }
}
