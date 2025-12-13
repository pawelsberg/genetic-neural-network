namespace Pawelsberg.GeneticNeuralNetwork.Model.Genetics.Mutating;

public class MultipleTimesMutator<TSpecimen> : Mutator<TSpecimen> where TSpecimen : ISpecimen
{
    // TODO - support also references to parent mutators 
    public Mutators<TSpecimen> Mutators { get; private set; }
    private readonly int _count;
    public MultipleTimesMutator(Mutators<TSpecimen> mutators, int count)
    {
        Mutators = mutators;
        _count = count;
    }

    public override MutationDescription Mutate(TSpecimen specimen)
    {
        MutationDescription mutationDescription = new MutationDescription();
        mutationDescription.Text = string.Format("MultipleTimes:Running {0} mutations:(", _count);
        for (int i = 0; i < _count; i++)
        {
            MutationDescription subMutationDescription = Mutators.Mutate(specimen);
            mutationDescription.Text += (i == 0 ? "" : ",") + subMutationDescription.Text;
        }
        mutationDescription.Text += ")";
        return mutationDescription;
    }
}
