namespace Pawelsberg.GeneticNeuralNetworkConsole.Model.Top;

public class ScrollingPanel
{
    public int Offset { get; private set; }

    public void Reset() => Offset = 0;

    public bool HandleKey(ConsoleKey key, int totalLines, int availableLines)
    {
        int maxOffset = Math.Max(0, totalLines - availableLines);
        int previousOffset = Offset;

        if (key == ConsoleKey.UpArrow && Offset > 0)
            Offset--;
        else if (key == ConsoleKey.DownArrow && Offset < maxOffset)
            Offset++;
        else if (key == ConsoleKey.PageUp)
            Offset = Math.Max(0, Offset - availableLines);
        else if (key == ConsoleKey.PageDown)
            Offset = Math.Min(maxOffset, Offset + availableLines);
        else if (key == ConsoleKey.Home)
            Offset = 0;
        else if (key == ConsoleKey.End)
            Offset = maxOffset;

        return Offset != previousOffset;
    }

    public void Render(IReadOnlyList<string> lines, int availableLines, int maxWidth)
    {
        int startLine = Offset;
        int endLine = Math.Min(startLine + availableLines, lines.Count);

        for (int i = startLine; i < endLine; i++)
        {
            if (!CanWriteLine()) return;
            string line = lines[i];
            string truncatedLine = line.Length > maxWidth ? line.Substring(0, maxWidth) : line;
            Console.WriteLine(truncatedLine.PadRight(maxWidth));
        }
    }

    public void Render(IReadOnlyList<string> lines, int availableLines, int maxWidth, Func<int, ConsoleColor> colorSelector)
    {
        int startLine = Offset;
        int endLine = Math.Min(startLine + availableLines, lines.Count);

        for (int i = startLine; i < endLine; i++)
        {
            if (!CanWriteLine()) return;
            Console.ForegroundColor = colorSelector(i);
            string line = lines[i];
            string truncatedLine = line.Length > maxWidth ? line.Substring(0, maxWidth) : line;
            Console.WriteLine(truncatedLine.PadRight(maxWidth));
        }
    }

    public void Render<T>(IReadOnlyList<T> items, int availableLines, int maxWidth, Action<T, int> renderAction)
    {
        int startLine = Offset;
        int endLine = Math.Min(startLine + availableLines, items.Count);

        for (int i = startLine; i < endLine; i++)
        {
            if (!CanWriteLine()) return;
            renderAction(items[i], maxWidth);
        }
    }

    private static bool CanWriteLine() => Console.CursorTop < Console.BufferHeight - 1;
}
