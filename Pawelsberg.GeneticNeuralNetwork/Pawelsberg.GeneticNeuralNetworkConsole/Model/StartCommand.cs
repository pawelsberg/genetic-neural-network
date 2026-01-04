using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model;

public class StartCommand : Command
{
    public static string Name = "start";
    public override void Run(NetworkSimulation simulation)
    {
        simulation.SimulationTimer.Start();
    }
    public override string ShortDescription { get { return "Start genetic algorithm simulation"; } }
}
