using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model.Top;

public static class Header
{
    public static int LineCount => 5;

    public static void Render(NetworkSimulation simulation, Mode currentMode)
    {
        int maxWidth = Console.WindowWidth - 1;
        if (maxWidth < 1) return;

        Console.ForegroundColor = ConsoleColor.White;
        string line1 = $"Generation: {simulation.GenerationNumber,-12} " +
                       $"Quality: {simulation.BestEverQuality:F13} / {simulation.MaxPossibleQuality:F2}  " +
                       $"[{(simulation.SimulationTimer.Running ? "RUNNING" : "PAUSED ")}]";
        if (!CanWriteLine()) return;
        Console.WriteLine(Truncate(line1, maxWidth));

        Console.ForegroundColor = ConsoleColor.Gray;
        int nodeCount = simulation.BestEver?.Nodes.Count ?? 0;
        int synapseCount = simulation.BestEver?.GetAllSynapses().Count() ?? 0;
        string line2 = $"Specimens: {simulation.MaxSpecimens,-6} " +
                       $"Nodes: {nodeCount,-6} Synapses: {synapseCount,-6} " +
                       $"Propagations: {simulation.Propagations}";
        if (!CanWriteLine()) return;
        Console.WriteLine(Truncate(line2, maxWidth));

        string line3 = $"Delay: {simulation.SimulationTimer.DelayTimeMs}ms  " +
                       $"MaxNodes: {simulation.MaxNodes}  MaxSynapses: {simulation.MaxSynapses}  " +
                       $"GenMultiplier: {simulation.GenerationMultiplier}";
        if (!CanWriteLine()) return;
        Console.WriteLine(Truncate(line3, maxWidth));

        Console.ForegroundColor = ConsoleColor.DarkGray;
        string modeKeys = string.Join("/", Modes.All.Select(m => m.Key.ToString().ToLowerInvariant()));
        string keyHintsPart = string.IsNullOrEmpty(currentMode.KeyHints) ? "" : $"  {currentMode.KeyHints}";
        string line4 = $"Mode: {currentMode.KeyIndicator}  Keys: {modeKeys}=switch{keyHintsPart}  q=quit";
        if (!CanWriteLine()) return;
        Console.WriteLine(Truncate(line4, maxWidth));

        Console.ForegroundColor = ConsoleColor.DarkCyan;
        if (!CanWriteLine()) return;
        Console.WriteLine(new string('-', maxWidth));
    }

    private static string Truncate(string text, int maxWidth)
    {
        string padded = text.PadRight(maxWidth);
        return padded.Length > maxWidth ? padded.Substring(0, maxWidth) : padded;
    }

    private static bool CanWriteLine() => Console.CursorTop < Console.BufferHeight - 1;
}
