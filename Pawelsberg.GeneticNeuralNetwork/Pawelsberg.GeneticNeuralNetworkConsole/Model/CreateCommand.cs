using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model;

public class CreateCommand : Command
{
    public static string Name = "create";
    public override void Run(NetworkSimulation simulation)
    {
        Network network = simulation.TestCaseList.CreateNetwork();
        network.ToString();
        simulation.Add(network);
    }
    public override string ShortDescription { get { return "Creates a neural network that fits test case list"; } }
}
