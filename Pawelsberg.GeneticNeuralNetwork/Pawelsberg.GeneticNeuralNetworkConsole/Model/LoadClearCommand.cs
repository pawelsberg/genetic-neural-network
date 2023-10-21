using Pawelsberg.GeneticNeuralNetwork.Model;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking.DiskStoring;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model;

public class LoadClearCommand : Command
{
    private string _name;
    public static string Name = "loadclear";
    public override void LoadParameters(CodedText text)
    {
        _name = text.ReadString();
        text.SkipWhiteCharacters();
        if (!text.EOT)
        {
            throw new Exception("Load clear command syntax exception - too many parameters");
        }
    }
    public override void Run(NetworkSimulation simulation)
    {
        Network network = NetworkList.LoadNetwork(_name);
        network.ToString();
        simulation.Replace(network);
    }
    public override string ShortDescription { get { return "Loads specific neural network as the only specimen in generation. Use 'pass' network as a parameter to load the simplest network (and restart algorithm)."; } }
}
