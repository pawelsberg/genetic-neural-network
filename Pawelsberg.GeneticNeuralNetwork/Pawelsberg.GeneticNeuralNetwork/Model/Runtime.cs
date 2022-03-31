namespace Pawelsberg.GeneticNeuralNetwork.Model
{
    public static class Runtime
    {
        public static bool IsRunningOnMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }
    }
}
