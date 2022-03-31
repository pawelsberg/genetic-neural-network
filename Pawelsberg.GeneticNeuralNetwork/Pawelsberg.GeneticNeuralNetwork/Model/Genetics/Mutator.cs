namespace Pawelsberg.GeneticNeuralNetwork.Model.Genetics
{
    public abstract class Mutator<TSpecimen> where TSpecimen : ISpecimen
    {
        public abstract MutationDescription Mutate(TSpecimen network);
    }
}
