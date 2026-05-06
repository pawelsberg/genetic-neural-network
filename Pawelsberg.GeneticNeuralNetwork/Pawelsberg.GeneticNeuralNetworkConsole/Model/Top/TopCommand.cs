using Pawelsberg.GeneticNeuralNetwork.Model;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model.Top;

public class TopCommand : Command
{
    public static string Name = "top";

    private Mode _initialMode = Modes.Default;

    public override void LoadParameters(CodedText text)
    {
        if (!text.EOT)
        {
            string modeText = text.ReadString();
            Mode mode = Modes.GetByName(modeText);
            if (mode == null)
            {
                string modeNames = string.Join("|", Modes.All.Select(m => m.Name.ToLowerInvariant()));
                throw new Exception($"Usage: top [{modeNames}]");
            }
            _initialMode = mode;
        }
    }

    public override void Run(NetworkSimulation simulation)
    {
        Mode currentMode = _initialMode;
        ScrollingPanel scrollingPanel = new ScrollingPanel();
        DateTime lastRefresh = DateTime.MinValue;
        int refreshIntervalMs = 500;
        bool needsFullRedraw = true;
        int lastWindowWidth = Console.WindowWidth;
        int lastWindowHeight = Console.WindowHeight;

        Console.CursorVisible = false;
        Console.Clear();

        try
        {
            while (true)
            {
                if (Console.WindowWidth != lastWindowWidth || Console.WindowHeight != lastWindowHeight)
                {
                    lastWindowWidth = Console.WindowWidth;
                    lastWindowHeight = Console.WindowHeight;
                    Console.Clear();
                    needsFullRedraw = true;
                }

                bool shouldRefresh = (DateTime.Now - lastRefresh).TotalMilliseconds >= refreshIntervalMs;

                if (shouldRefresh || needsFullRedraw)
                {
                    RenderScreen(simulation, currentMode, scrollingPanel, needsFullRedraw);
                    lastRefresh = DateTime.Now;
                    needsFullRedraw = false;
                }

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(intercept: true);

                    if (key.Key == ConsoleKey.Q)
                        break;

                    Mode newMode = Modes.GetByKey(key.Key);

                    if (newMode != null && newMode != currentMode)
                    {
                        currentMode = newMode;
                        scrollingPanel.Reset();
                        needsFullRedraw = true;
                    }

                    int availableLines = Console.WindowHeight - Header.LineCount - 1;
                    if (currentMode.HandleKey(key.Key, simulation, scrollingPanel, availableLines))
                        needsFullRedraw = true;
                }
                else
                {
                    Thread.Sleep(20);
                }
            }
        }
        finally
        {
            Console.CursorVisible = true;
            Console.ResetColor();
            Console.Clear();
        }
    }

    private void RenderScreen(NetworkSimulation simulation, Mode currentMode,
        ScrollingPanel scrollingPanel, bool fullRedraw)
    {
        int windowHeight = Console.WindowHeight;
        int windowWidth = Console.WindowWidth;
        int bufferHeight = Console.BufferHeight;

        if (windowHeight < Header.LineCount + 2 || windowWidth < 20)
            return;

        SafeSetCursorPosition(0, 0);

        Header.Render(simulation, currentMode);

        int availableLines = windowHeight - Header.LineCount - 1;

        if (fullRedraw)
            ClearContentArea(Header.LineCount, availableLines, windowWidth);

        SafeSetCursorPosition(0, Header.LineCount);

        currentMode.Render(simulation, availableLines, scrollingPanel);

        ClearRemainingLines(windowHeight, windowWidth);
    }

    private void SafeSetCursorPosition(int left, int top)
    {
        try
        {
            int currentBufferHeight = Console.BufferHeight;
            int currentBufferWidth = Console.BufferWidth;
            if (top >= 0 && top < currentBufferHeight && left >= 0 && left < currentBufferWidth)
                Console.SetCursorPosition(left, top);
        }
        catch (ArgumentOutOfRangeException)
        {
            // Buffer size changed between check and set - ignore
        }
    }

    private void ClearContentArea(int startLine, int lineCount, int windowWidth)
    {
        string emptyLine = new string(' ', Math.Max(0, windowWidth - 1));
        for (int i = 0; i < lineCount; i++)
        {
            SafeSetCursorPosition(0, startLine + i);
            SafeWrite(emptyLine);
        }
    }

    private void ClearRemainingLines(int windowHeight, int windowWidth)
    {
        int currentTop = Console.CursorTop;
        int bufferHeight = Console.BufferHeight;
        if (currentTop >= windowHeight || currentTop >= bufferHeight)
            return;

        string emptyLine = new string(' ', Math.Max(0, windowWidth - 1));
        int maxLine = Math.Min(windowHeight - 1, bufferHeight - 1);

        for (int i = currentTop; i < maxLine; i++)
        {
            SafeSetCursorPosition(0, i);
            SafeWrite(emptyLine);
        }
    }

    private static void SafeWrite(string text)
    {
        try
        {
            Console.Write(text);
        }
        catch (ArgumentOutOfRangeException)
        {
            // Buffer size changed - ignore
        }
    }

    public override string ShortDescription =>
        $"Live view of simulation state. Modes: {string.Join(", ", Modes.All.Select(m => m.Name.ToLowerInvariant()))}.\n" +
        $" Keys: {string.Join("/", Modes.All.Select(m => m.Key.ToString().ToLowerInvariant()))}=switch mode, q=quit";

    public override IEnumerable<string> GetParameterCompletions(string[] parameters)
    {
        if (parameters.Length == 0)
            return Modes.All.Select(m => m.Name.ToLowerInvariant());
        return Enumerable.Empty<string>();
    }
}
