using Pawelsberg.GeneticNeuralNetwork.Model;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking.DiskStoring;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model;

public class SaveCommand : Command
{
    private string _name;
    public static string Name = "save";
    public override void LoadParameters(CodedText text)
    {
        _name = text.ReadString();
        text.SkipWhiteCharacters();
        if (!text.EOT)
        {
            throw new Exception("Save command syntax exception - too many parameters");
        }
    }
    public override void Run(NetworkSimulation simulation)
    {
        NetworkList.Save(simulation.BestEver, _name);
    }
    public override string ShortDescription { get { return "Save the best neural network"; } }
}

