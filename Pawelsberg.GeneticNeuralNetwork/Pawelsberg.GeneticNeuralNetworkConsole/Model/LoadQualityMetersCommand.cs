using Pawelsberg.GeneticNeuralNetwork.Model;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring.DiskStoring;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model;

public class LoadQualityMetersCommand : Command
{
    private string _name;
    public static string Name = "loadqm";

    public override void LoadParameters(CodedText text)
    {
        if (text.EOT)
            throw new Exception("LoadQualityMeters command syntax exception - quality meters name required");
        _name = text.ReadString();
        text.SkipWhiteCharacters();
        if (!text.EOT)
            throw new Exception("LoadQualityMeters command syntax exception - too many parameters");
    }

    public override void Run(NetworkSimulation simulation)
    {
        simulation.QualityMeterFactory = (propagations, testCaseList) =>
            NetworkQualityMetersList.LoadNetworkQualityMeters(_name, propagations, testCaseList);
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine($"Loaded quality meters from {_name}");
    }

    public override string ShortDescription { get { return "Loads quality meters from file (e.g. 'loadqm Normal')"; } }
}
