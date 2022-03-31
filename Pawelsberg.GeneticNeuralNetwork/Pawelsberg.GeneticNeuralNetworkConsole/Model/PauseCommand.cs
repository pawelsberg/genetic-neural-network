using Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model;

public class PauseCommand : Command
{
    public static string Name = "pause";
    public override void Run(NetworkSimulation simulation)
    {
        simulation.SimulationTimer.Pause();
    }
    public override string ShortDescription { get { return "Pause genetic algorithm"; } }
}

