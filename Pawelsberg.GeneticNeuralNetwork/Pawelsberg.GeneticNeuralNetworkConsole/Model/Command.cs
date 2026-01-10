using Pawelsberg.GeneticNeuralNetwork.Model;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model;

public abstract class Command
{
    public virtual void Run(NetworkSimulation simulation) { }
    public virtual void LoadParameters(CodedText text) { }
    public virtual string ShortDescription { get { return string.Empty; } }
    public virtual IEnumerable<string> GetParameterCompletions(string[] parameters) => Enumerable.Empty<string>();
}


