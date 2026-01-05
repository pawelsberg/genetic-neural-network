namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.Mutating;

/// <summary>
/// Interface for network mutators that have a maximum node count constraint.
/// </summary>
public interface IMaxNodesLimitedMutator
{
    int MaxNodes { get; set; }
}
