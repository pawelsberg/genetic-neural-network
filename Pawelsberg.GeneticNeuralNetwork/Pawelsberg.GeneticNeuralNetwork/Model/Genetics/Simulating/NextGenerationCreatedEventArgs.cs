namespace Pawelsberg.GeneticNeuralNetwork.Model.Genetics.Simulating;

public class NextGenerationCreatedEventArgs<TSpecimen> : EventArgs where TSpecimen : ISpecimen
{
    public Generation<TSpecimen> NextGeneration { get; private set; }
    public Generation<TSpecimen> PrevGeneration { get; private set; }
    public bool Pause { get; set; }
    public NextGenerationCreatedEventArgs(Generation<TSpecimen> nextGeneration, Generation<TSpecimen> prevGeneration) : base()
    {
        NextGeneration = nextGeneration;
        PrevGeneration = prevGeneration;
    }
}

