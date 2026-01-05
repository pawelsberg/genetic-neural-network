namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.Mutating;

/// <summary>
/// Interface for network mutators that have a maximum synapse count constraint.
/// </summary>
public interface IMaxSynapsesLimitedMutator
{
    int MaxSynapses { get; set; }
}
