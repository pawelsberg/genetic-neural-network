namespace Pawelsberg.GeneticNeuralNetworkConsole.Model.Top;

public static class Modes
{
    public static IReadOnlyList<Mode> All { get; } = new List<Mode>
    {
        new TestResultsMode(),
        new MutationsMode(),
        new NetworkViewMode()
    };

    public static Mode Default => All[0];

    public static Mode? GetByKey(ConsoleKey key)
        => All.FirstOrDefault(m => m.Key == key);

    public static Mode? GetByName(string name)
        => All.FirstOrDefault(m => m.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase));
}
