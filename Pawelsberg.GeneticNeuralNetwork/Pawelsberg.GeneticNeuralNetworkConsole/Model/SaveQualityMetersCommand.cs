using Pawelsberg.GeneticNeuralNetwork.Model;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting.QualityMeasuring.DiskStoring;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model;

public class SaveQualityMetersCommand : Command
{
    private string _name;
    public static string Name = "saveqm";

    public override void LoadParameters(CodedText text)
    {
        if (text.EOT)
            throw new Exception("SaveQualityMeters command syntax exception - quality meters name required");
        _name = text.ReadString();
        text.SkipWhiteCharacters();
        if (!text.EOT)
            throw new Exception("SaveQualityMeters command syntax exception - too many parameters");
    }

    public override void Run(NetworkSimulation simulation)
    {
        NetworkQualityMetersList.Save(simulation.GenerationMeter, _name);
        Console.WriteLine($"Saved current quality meters.");
    }

    public override string ShortDescription { get { return "Saves current quality meters to file (e.g. 'saveqm MyMeters')"; } }
}
