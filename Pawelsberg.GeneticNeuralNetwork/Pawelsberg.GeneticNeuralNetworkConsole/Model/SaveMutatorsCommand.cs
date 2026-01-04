using Pawelsberg.GeneticNeuralNetwork.Model;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.Mutating;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.Mutating.DiskStoring;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model;

public class SaveMutatorsCommand : Command
{
    private string _name;
    public static string Name = "savemutators";

    public override void LoadParameters(CodedText text)
    {
        if (text.EOT)
            throw new Exception("SaveMutators command syntax exception - mutators name required");
        _name = text.ReadString();
        text.SkipWhiteCharacters();
        if (!text.EOT)
            throw new Exception("SaveMutators command syntax exception - too many parameters");
    }

    public override void Run(NetworkSimulation simulation)
    {
        NetworkMutators? mutators = simulation.Mutators as NetworkMutators;
        if (mutators == null)
            throw new Exception("Current mutators cannot be saved - not a NetworkMutators instance");

        NetworkMutatorsList.Save(mutators, _name);
        Console.WriteLine($"Saved current mutators.");
    }

    public override string ShortDescription { get { return "Saves current mutators to file (e.g. 'savemutators MyMutators')"; } }
}
