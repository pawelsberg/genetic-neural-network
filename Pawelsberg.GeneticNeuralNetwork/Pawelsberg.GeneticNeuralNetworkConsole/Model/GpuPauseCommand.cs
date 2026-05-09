using Pawelsberg.GeneticNeuralNetwork.Gpu;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model;

public class GpuPauseCommand : Command
{
    public static string Name = "gpupause";

    public override void Run(NetworkSimulation simulation)
    {
        GpuSimulation? gpuSim = GpuSimulationProvider.GetExisting();
        if (gpuSim == null)
        {
            Console.WriteLine("GPU simulation not started.");
            return;
        }
        gpuSim.Pause();
        // Push the freshest BestEver into the CPU simulation (the periodic-sync callback
        // adds it via simulation.Add). Final sync ensures the CPU sees what the GPU has
        // when the user pauses, even if the most recent periodic tick was many generations
        // ago. Downloads + invokes the callback on the worker thread; blocks until done.
        gpuSim.SyncNow();
        Console.WriteLine($"GPU simulation paused at generation {gpuSim.GenerationNumber}. Best fitness: {gpuSim.BestEverFitness:F6}");
    }

    public override string ShortDescription { get { return "Pause the GPU genetic algorithm simulation"; } }
}
