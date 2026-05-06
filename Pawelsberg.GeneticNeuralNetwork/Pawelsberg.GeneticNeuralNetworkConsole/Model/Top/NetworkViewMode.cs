using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model.Top;

public class NetworkViewMode : Mode
{
    public override string Name => "Network";
    public override string KeyIndicator => "[N]etwork";
    public override ConsoleKey Key => ConsoleKey.N;
    public override string KeyHints => "Up/Dn/PgUp/PgDn/Home/End=scroll";

    public override int GetTotalLines(NetworkSimulation simulation)
    {
        if (simulation.BestEver == null)
            return 1;
        return simulation.BestEver.ToString().Split('\n').Length + 1; // +1 for header line
    }

    public override bool HandleKey(ConsoleKey key, NetworkSimulation simulation, ScrollingPanel scrollingPanel, int availableLines)
    {
        int totalLines = GetTotalLines(simulation);
        return scrollingPanel.HandleKey(key, totalLines, availableLines);
    }

    public override void Render(NetworkSimulation simulation, int availableLines, ScrollingPanel scrollingPanel)
    {
        if (simulation.BestEver == null)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            if (CanWriteLine())
                Console.WriteLine("No network available.");
            return;
        }

        string networkText = simulation.BestEver.ToString();
        List<string> lines = networkText.Split('\n').Select(l => l.TrimEnd()).ToList();
        int maxWidth = Console.WindowWidth - 1;

        int linesForContent = availableLines - 1; // Reserve one line for header
        int startLine = scrollingPanel.Offset;
        int endLine = Math.Min(startLine + linesForContent, lines.Count);

        Console.ForegroundColor = ConsoleColor.Cyan;
        if (!CanWriteLine()) return;
        Console.WriteLine($"Best Network (lines {startLine + 1}-{endLine} of {lines.Count}):".PadRight(maxWidth));
        Console.ForegroundColor = ConsoleColor.Gray;

        scrollingPanel.Render(lines, linesForContent, maxWidth);
    }

    private static bool CanWriteLine() => Console.CursorTop < Console.BufferHeight - 1;
}
