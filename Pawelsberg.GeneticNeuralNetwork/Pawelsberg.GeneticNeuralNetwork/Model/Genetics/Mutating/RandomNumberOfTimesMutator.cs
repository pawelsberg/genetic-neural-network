namespace Pawelsberg.GeneticNeuralNetwork.Model.Genetics.Mutating;

public class RandomNumberOfTimesMutator<TSpecimen> : Mutator<TSpecimen> where TSpecimen : ISpecimen
{
    public Mutator<TSpecimen> Mutator { get; private set; }
    public int MinNumberOfTimes { get; private set; }
    public int MaxNumberOfTimes { get; private set; }

    public RandomNumberOfTimesMutator(Mutator<TSpecimen> mutator, int minNumberOfTimes, int maxNumberOfTimes)
    {
        Mutator = mutator;
        MinNumberOfTimes = minNumberOfTimes;
        MaxNumberOfTimes = maxNumberOfTimes;
    }

    public override MutationDescription Mutate(TSpecimen specimen)
    {
        MutationDescription mutationDescription = new MutationDescription();
        int count = RandomGenerator.Random.Next(MinNumberOfTimes, MaxNumberOfTimes + 1);
        mutationDescription.Text = string.Format("RandomNumberOfTimes({0}):(", count);

        for (int i = 0; i < count; i++)
        {
            MutationDescription subMutationDescription = Mutator.Mutate(specimen);
            mutationDescription.Text += (i == 0 ? "" : ",") + subMutationDescription.Text;
        }
        mutationDescription.Text += ")";
        return mutationDescription;
    }

}
