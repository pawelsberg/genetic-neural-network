using System.Diagnostics;
using Pawelsberg.GeneticNeuralNetwork.Gpu;
using Pawelsberg.GeneticNeuralNetwork.Model;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model;

public class GpuRunCommand : Command
{
    public static string Name = "gpurun";
    private int? _generationCount;

    public override void LoadParameters(CodedText text)
    {
        if (!text.EOT)
        {
            int n = text.ReadInt();
            if (n < 1)
                throw new Exception("generation count should be greater than 0");
            _generationCount = n;
            text.SkipWhiteCharacters();
        }
        if (!text.EOT)
            throw new Exception("Usage: gpurun [generations]");
    }

    public override void Run(NetworkSimulation simulation)
    {
        int generations = _generationCount ?? GpuConstants.Generations;
        GpuSimulation gpuSim = GpuSimulationProvider.GetOrCreate(simulation);
        Stopwatch sw = Stopwatch.StartNew();
        gpuSim.EnsureStarted();
        Console.WriteLine($"GPU device: {gpuSim.DeviceInfo}");
        Console.WriteLine($"Setup: {sw.ElapsedMilliseconds} ms. Running {generations} generation(s) on GPU... (Ctrl+C to interrupt)");

        int startGen = gpuSim.GenerationNumber;
        int targetGen = startGen + generations;
        int progressInterval = gpuSim.PeriodicSyncInterval;

        Cancellation.Reset();
        sw.Restart();
        gpuSim.RunUntil(targetGen);
        // Align progress reports with the periodic sync — that's when BestEverFitness
        // is actually fresh (worker doesn't download per step to keep GPU saturated).
        int nextProgressGen = startGen + progressInterval - (startGen % progressInterval);
        while (gpuSim.IsRunning && !Cancellation.Requested)
        {
            Thread.Sleep(50);
            int now = gpuSim.GenerationNumber;
            if (now >= nextProgressGen && now < targetGen)
            {
                Console.WriteLine($"  gen {now,5}/{targetGen}  bestFitness={gpuSim.BestEverFitness:F6}");
                nextProgressGen = now + progressInterval;
            }
        }
        gpuSim.Pause();
        // Force a final sync so the CPU sim sees the freshest BestEver even if the run
        // ended between two periodic-sync ticks (or was interrupted by Ctrl+C).
        gpuSim.SyncNow();
        sw.Stop();

        string suffix = Cancellation.Requested ? " (interrupted)" : "";
        Console.WriteLine($"Done in {sw.Elapsed.TotalSeconds:F2} s at generation {gpuSim.GenerationNumber}{suffix}. Best fitness: {gpuSim.BestEverFitness:F6}");
    }

    public override string ShortDescription { get { return $"Run N generations on the GPU (default {GpuConstants.Generations}). Best is added to CPU sim periodically and on completion."; } }
}
