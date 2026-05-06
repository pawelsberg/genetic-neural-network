using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model.Top;

public class TestResultsMode : Mode
{
    private int _actualPrecision = 2;
    private static readonly int[] PrecisionValues = { 0, 2, 4, 6, 8, 10, 13 };

    public override string Name => "TestResults";
    public override string KeyIndicator => "[T]ests";
    public override ConsoleKey Key => ConsoleKey.T;
    public override string KeyHints => "Up/Dn/PgUp/PgDn/Home/End=scroll, p=precision";

    public override bool HandleKey(ConsoleKey key, NetworkSimulation simulation, ScrollingPanel scrollingPanel, int availableLines)
    {
        if (key == ConsoleKey.P)
        {
            int currentIndex = Array.IndexOf(PrecisionValues, _actualPrecision);
            int nextIndex = (currentIndex + 1) % PrecisionValues.Length;
            _actualPrecision = PrecisionValues[nextIndex];
            return true;
        }
        int totalLines = GetTotalLines(simulation);
        return scrollingPanel.HandleKey(key, totalLines, availableLines);
    }

    public override int GetTotalLines(NetworkSimulation simulation)
    {
        if (simulation.TestCaseList == null)
            return 1;
        return simulation.TestCaseList.TestCases.Count + 1; // +1 for header
    }

    public override void Render(NetworkSimulation simulation, int availableLines, ScrollingPanel scrollingPanel)
    {
        if (simulation.TestCaseList == null)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            if (CanWriteLine())
                Console.WriteLine("No test cases loaded.");
            return;
        }

        int maxWidth = Console.WindowWidth - 1;
        Network bestEver = simulation.BestEver;
        TestResultData data = BuildTestResultData(simulation, bestEver);

        Console.ForegroundColor = ConsoleColor.Cyan;
        if (!CanWriteLine()) return;
        Console.WriteLine(FormatHeader(data).PadRight(maxWidth));
        Console.ForegroundColor = ConsoleColor.Gray;

        scrollingPanel.Render(data.Lines, availableLines - 1, maxWidth, (line, width) => RenderLine(line, width, data));
    }

    private static bool CanWriteLine() => Console.CursorTop < Console.BufferHeight - 1;

    private string FormatHeader(TestResultData data)
    {
        int inputsTotalWidth = data.InputWidths.Sum() + Math.Max(0, data.InputWidths.Count - 1);
        int expectedTotalWidth = data.ExpectedWidths.Sum() + Math.Max(0, data.ExpectedWidths.Count - 1);
        int actualTotalWidth = data.ActualWidths.Sum() + Math.Max(0, data.ActualWidths.Count - 1);
        
        string inputsHeader = TruncateLabel("Inputs", inputsTotalWidth);
        string expectedHeader = TruncateLabel("Expected", expectedTotalWidth);
        string actualHeader = TruncateLabel($"Actual(F{_actualPrecision})", actualTotalWidth);
        
        return $"{"#".PadLeft(data.IndexWidth)}: {inputsHeader} -> {expectedHeader} = {actualHeader}";
    }

    private static string TruncateLabel(string label, int width)
    {
        if (width <= 0)
            return "";
        if (label.Length <= width)
            return label.PadLeft(width);
        return label.Substring(0, width);
    }

    private void RenderLine(TestResultLine line, int maxWidth, TestResultData data)
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write($"{line.Index.ToString().PadLeft(data.IndexWidth)}: ");

        for (int i = 0; i < data.InputWidths.Count; i++)
        {
            if (i > 0) Console.Write(" ");
            int? value = i < line.Inputs.Count ? line.Inputs[i] : null;
            Console.Write(FormatInt(value).PadLeft(data.InputWidths[i]));
        }

        Console.Write(" -> ");

        for (int i = 0; i < data.ExpectedWidths.Count; i++)
        {
            if (i > 0) Console.Write(" ");
            int? value = i < line.Expected.Count ? line.Expected[i] : null;
            Console.Write(FormatInt(value).PadLeft(data.ExpectedWidths[i]));
        }

        Console.Write(" = ");

        for (int i = 0; i < data.ActualWidths.Count; i++)
        {
            if (i > 0) Console.Write(" ");
            double? value = i < line.Actual.Count ? line.Actual[i] : null;
            bool isMismatch = i < line.Mismatches.Count && line.Mismatches[i];
            Console.ForegroundColor = isMismatch ? ConsoleColor.Red : ConsoleColor.Gray;
            Console.Write(FormatActual(value).PadLeft(data.ActualWidths[i]));
        }

        Console.ForegroundColor = ConsoleColor.Gray;
        int currentLength = CalculateLineLength(line, data);
        if (currentLength < maxWidth)
            Console.Write(new string(' ', maxWidth - currentLength));
        Console.WriteLine();
    }

    private int CalculateLineLength(TestResultLine line, TestResultData data)
    {
        int length = data.IndexWidth + 2; // index + ": "
        length += data.InputWidths.Sum() + Math.Max(0, data.InputWidths.Count - 1); // inputs with spaces
        length += 4; // " -> "
        length += data.ExpectedWidths.Sum() + Math.Max(0, data.ExpectedWidths.Count - 1); // expected with spaces
        length += 3; // " = "
        length += data.ActualWidths.Sum() + Math.Max(0, data.ActualWidths.Count - 1); // actual with spaces
        return length;
    }

    private string FormatInt(int? value)
        => value.HasValue ? value.Value.ToString() : "";

    private string FormatActual(double? value)
        => value.HasValue ? value.Value.ToString($"F{_actualPrecision}") : "";

    private TestResultData BuildTestResultData(NetworkSimulation simulation, Network bestEver)
    {
        TestResultData data = new TestResultData();
        int testCount = simulation.TestCaseList.TestCases.Count;
        data.IndexWidth = Math.Max(1, testCount.ToString().Length);

        int testIndex = 0;
        foreach (TestCase testCase in simulation.TestCaseList.TestCases)
        {
            testIndex++;
            TestResultLine line = new TestResultLine { Index = testIndex };

            for (int i = 0; i < testCase.Inputs.Count; i++)
            {
                int input = testCase.Inputs[i];
                line.Inputs.Add(input);
                EnsureWidth(data.InputWidths, i, FormatInt(input).Length);
            }

            for (int i = 0; i < testCase.Outputs.Count; i++)
            {
                int expected = testCase.Outputs[i];
                line.Expected.Add(expected);
                EnsureWidth(data.ExpectedWidths, i, FormatInt(expected).Length);
            }

            if (bestEver != null)
            {
                try
                {
                    RunningContext runningContext = bestEver.SafeRun(testCase, simulation.Propagations);
                    for (int i = 0; i < testCase.Outputs.Count; i++)
                    {
                        int expected = testCase.Outputs[i];
                        double actual = runningContext.GetPotential(bestEver.Outputs[i]);
                        bool isMismatch = Math.Abs(actual - expected) > 0.001;
                        line.Actual.Add(actual);
                        line.Mismatches.Add(isMismatch);
                        EnsureWidth(data.ActualWidths, i, FormatActual(actual).Length);
                    }
                }
                catch
                {
                    for (int i = 0; i < testCase.Outputs.Count; i++)
                    {
                        line.Actual.Add(null);
                        line.Mismatches.Add(true);
                        EnsureWidth(data.ActualWidths, i, FormatActual(null).Length);
                    }
                }
            }
            else
            {
                for (int i = 0; i < testCase.Outputs.Count; i++)
                {
                    line.Actual.Add(null);
                    line.Mismatches.Add(false);
                    EnsureWidth(data.ActualWidths, i, FormatActual(null).Length);
                }
            }

            data.Lines.Add(line);
        }

        return data;
    }

    private void EnsureWidth(List<int> widths, int index, int length)
    {
        while (widths.Count <= index)
            widths.Add(0);
        widths[index] = Math.Max(widths[index], length);
    }

    private class TestResultData
    {
        public int IndexWidth { get; set; }
        public List<int> InputWidths { get; set; } = new List<int>();
        public List<int> ExpectedWidths { get; set; } = new List<int>();
        public List<int> ActualWidths { get; set; } = new List<int>();
        public List<TestResultLine> Lines { get; } = new List<TestResultLine>();
    }

    private class TestResultLine
    {
        public int Index { get; set; }
        public List<int> Inputs { get; } = new List<int>();
        public List<int> Expected { get; } = new List<int>();
        public List<double?> Actual { get; } = new List<double?>();
        public List<bool> Mismatches { get; } = new List<bool>();
    }
}
