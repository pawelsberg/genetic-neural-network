namespace Pawelsberg.GeneticNeuralNetwork.Model.Genetics.Mutating
{
    public class NothingDoerMutator<TSpecimen> : Mutator<TSpecimen> where TSpecimen : ISpecimen
    {
        public override MutationDescription Mutate(TSpecimen specimen)
        {
            return new MutationDescription() { Text = "NothingDoer" };
        }
    }
}