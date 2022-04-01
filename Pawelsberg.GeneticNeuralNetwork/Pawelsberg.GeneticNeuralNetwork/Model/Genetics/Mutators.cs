namespace Pawelsberg.GeneticNeuralNetwork.Model.Genetics;

public class Mutators<TSpecimen> where TSpecimen : ISpecimen
{
    public List<double> MutatorWeights { get; private set; }
    public List<Mutator<TSpecimen>> MutatorList { get; private set; }

    public Mutators()
    {
        MutatorList = new List<Mutator<TSpecimen>>();
        MutatorWeights = new List<double>();
    }

    public void Add(Mutator<TSpecimen> mutator, double weight)
    {
        if (weight < 0)
            throw new ArgumentException("Have to be greater or equal to 0", "weight");
        MutatorList.Add(mutator);
        MutatorWeights.Add(weight);
    }
    public void Remove(Mutator<TSpecimen> mutator)
    {
        int indexOfMutator = MutatorList.IndexOf(mutator);
        MutatorList.RemoveAt(indexOfMutator);
        MutatorWeights.RemoveAt(indexOfMutator);
    }
    public void SetWeight(Mutator<TSpecimen> mutator, double weight)
    {
        int indexOfMutator = MutatorList.IndexOf(mutator);
        MutatorWeights[indexOfMutator] = weight;
    }
    public void Normalize()
    {
        double sum = MutatorWeights.Sum();
        if (sum <= 0d)
            return;

        for (int i = 0; i < MutatorWeights.Count; i++)
            MutatorWeights[i] /= sum;
    }
    public MutationDescription Mutate(TSpecimen specimen)
    {
        double sum = MutatorWeights.Sum();
        if (sum <= 0d)
            return new MutationDescription();

        double random = RandomGenerator.Random.NextDouble() * sum;

        for (int i = 0; i < MutatorWeights.Count; i++)
        {
            random -= MutatorWeights[i];
            if (random <= 0d)
            {
                return MutatorList[i].Mutate(specimen);
            }
        }
        return new MutationDescription();
    }
}
