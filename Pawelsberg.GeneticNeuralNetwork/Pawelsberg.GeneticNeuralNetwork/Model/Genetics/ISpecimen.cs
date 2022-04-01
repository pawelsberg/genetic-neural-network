namespace Pawelsberg.GeneticNeuralNetwork.Model.Genetics;

public interface ISpecimen
{
    ISpecimen DeepClone();
    MutationDescription MutationDescription { get; set; }
}
