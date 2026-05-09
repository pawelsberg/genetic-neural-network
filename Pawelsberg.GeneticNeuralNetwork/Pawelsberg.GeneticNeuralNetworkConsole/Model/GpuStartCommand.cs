using Pawelsberg.GeneticNeuralNetwork.Gpu;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model;

public class GpuStartCommand : Command
{
    public static string Name = "gpustart";

    public override void Run(NetworkSimulation simulation)
    {
        GpuSimulation gpuSim = GpuSimulationProvider.GetOrCreate(simulation);
        gpuSim.EnsureStarted();
        gpuSim.Start();
        // Quiet on success — printing nothing matches CPU `start`. The GLFW window is
        // created with StartFocused=false (see GlContext) so console focus is preserved.
    }

    public override string ShortDescription { get { return "Start the GPU genetic algorithm simulation in the background"; } }
}
