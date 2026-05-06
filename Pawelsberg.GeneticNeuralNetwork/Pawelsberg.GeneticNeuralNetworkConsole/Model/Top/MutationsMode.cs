using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model.Top;

public class MutationsMode : Mode
{
    public override string Name => "Mutations";
    public override string KeyIndicator => "[M]utations";
    public override ConsoleKey Key => ConsoleKey.M;
    public override string KeyHints => "";

    public override void Render(NetworkSimulation simulation, int availableLines, ScrollingPanel scrollingPanel)
    {
        List<string> logItems = simulation.Log.LogItems.ToList();

        if (logItems.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            if (CanWriteLine())
                Console.WriteLine("No mutations recorded yet.");
            return;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        if (!CanWriteLine()) return;
        Console.WriteLine($"Last {logItems.Count} successful mutations:");
        Console.ForegroundColor = ConsoleColor.Gray;

        List<string> lines = new List<string>();
        foreach (string mutation in logItems.AsEnumerable().Reverse())
        {
            string[] mutationLines = mutation.Split('\n');
            foreach (string line in mutationLines)
                lines.Add(line);
            lines.Add("");
        }

        int maxWidth = Console.WindowWidth - 1;
        scrollingPanel.Render(lines, availableLines - 1, maxWidth);
    }

    private static bool CanWriteLine() => Console.CursorTop < Console.BufferHeight - 1;
}
