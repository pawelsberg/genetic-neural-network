using Pawelsberg.GeneticNeuralNetwork.Gpu;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model;

public class CreateClearDummyCommand : Command
{
    public static string Name = "createcleardummy";
    public override void Run(NetworkSimulation simulation)
    {
        TestCaseList testCaseList = simulation.TestCaseList;
        int inputsCount = testCaseList.TestCases.Max(tc => tc.Inputs.Count);
        int outputsCount = testCaseList.TestCases.Max(tc => tc.Outputs.Count);

        Network network = new Network();

        List<Synapse> inputSynapses = Enumerable.Range(0, inputsCount).Select(i => new Synapse()).ToList();
        List<Synapse> outputSynapses = Enumerable.Range(0, outputsCount).Select(i => new Synapse()).ToList();
        network.Inputs.AddRange(inputSynapses);
        network.Outputs.AddRange(outputSynapses);

        Neuron neuron = new Neuron() { ActivationFunction = ActivationFunction.Linear };
        foreach (Synapse inputSynapse in inputSynapses)
            neuron.AddInput(inputSynapse, 1d);
        foreach (Synapse outputSynapse in outputSynapses)
            neuron.AddOutput(outputSynapse);
        network.Nodes.Add(neuron);

        // The standard InvalidatesGpuSimulation/Reload path syncs the GPU's BestEver
        // back into the CPU before disposing — that would defeat the clear, because
        // the GPU's evolved network would beat the dummy on the next tick. So drop
        // the GPU here without syncing, then apply the dummy synchronously so a
        // subsequent gpu* command rebuilds with the dummy as its seed.
        GpuSimulation? gpu = GpuSimulationProvider.GetExisting();
        bool gpuWasRunning = gpu != null && gpu.IsRunning;
        GpuSimulationProvider.DisposeIfAny();

        simulation.ReplaceImmediate(network);

        if (gpuWasRunning)
            GpuSimulationProvider.GetOrCreate(simulation).Start();
    }
    public override string ShortDescription { get { return "Clears the generation and BestEver and replaces it with a dummy network: one Linear neuron wired to every test-case input and output, all multipliers 1. Also drops any persistent GPU sim."; } }
    public override bool InvalidatesGpuSimulation => false;
}
