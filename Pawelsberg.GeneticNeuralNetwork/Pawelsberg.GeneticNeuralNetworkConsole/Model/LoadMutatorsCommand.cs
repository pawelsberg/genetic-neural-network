using Pawelsberg.GeneticNeuralNetwork.Model;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting.Mutating;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting.Mutating.DiskStoring;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model;

public class LoadMutatorsCommand : Command
{
    private string _name;
    public static string Name = "loadmutators";

    public override void LoadParameters(CodedText text)
    {
        if (text.EOT)
            throw new Exception("LoadMutators command syntax exception - mutators name required");
        _name = text.ReadString();
        text.SkipWhiteCharacters();
        if (!text.EOT)
            throw new Exception("LoadMutators command syntax exception - too many parameters");
    }

    public override void Run(NetworkSimulation simulation)
    {
        NetworkMutators mutators = NetworkMutatorsList.LoadNetworkMutators(_name, simulation.MaxNodes, simulation.MaxSynapses, simulation.Propagations, simulation.TestCaseList);
        simulation.Mutators = mutators;
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine($"Loaded mutators from {_name}");
    }

    public override string ShortDescription { get { return "Loads mutators from file (e.g. 'loadmutators Normal')"; } }
}
