namespace Pawelsberg.GeneticNeuralNetworkConsole.Model;

public class CommandInputCompletions
{
    private List<string> _currentCompletions;
    private int? _completionIndex; // Current position in completions list during tab cycling; null when not cycling

    public CommandInputCompletions()
    {
        _currentCompletions = new List<string>();
        _completionIndex = null;
    }

    public bool IsCycling => _completionIndex != null;

    public IReadOnlyList<string> CurrentCompletions => _currentCompletions;

    public void LoadCompletions(string input)
    {
        _currentCompletions = GetCompletions(input).ToList();
    }

    public string? TryCycleNext()
    {
        if (_currentCompletions.Count == 0)
            return null;

        if (_completionIndex == null)
            _completionIndex = 0;
        else
            _completionIndex = (_completionIndex.Value + 1) % _currentCompletions.Count;

        return _currentCompletions[_completionIndex.Value];
    }

    public string? TryCyclePrevious()
    {
        if (_currentCompletions.Count == 0)
            return null;

        if (_completionIndex == null)
            _completionIndex = _currentCompletions.Count - 1;
        else
            _completionIndex = (_completionIndex.Value - 1 + _currentCompletions.Count) % _currentCompletions.Count;

        return _currentCompletions[_completionIndex.Value];
    }

    public void Reset()
    {
        _completionIndex = null;
        _currentCompletions.Clear();
    }

    private static IEnumerable<string> GetCompletions(string input)
    {
        string trimmedInput = input.TrimStart();
        string[] parts = trimmedInput.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // No input yet - show all command names
        if (parts.Length == 0)
            return Commands.Names.OrderBy(k => k);

        string commandName = parts[0].ToLowerInvariant();
        string currentWord = parts.Length > 1 ? parts[^1] : (input.EndsWith(" ") ? "" : parts[0]);
        bool hasSpace = input.EndsWith(" ");

        // Still typing command name
        if (parts.Length == 1 && !hasSpace)
            return Commands.Names
                .Where(k => k.StartsWith(commandName, StringComparison.OrdinalIgnoreCase))
                .OrderBy(k => k);

        // Command has been typed, now completing parameters
        if (!Commands.NameCommands.ContainsKey(commandName))
            return Enumerable.Empty<string>();

        Command command = Commands.NameCommands[commandName]();
        string[] parameters = hasSpace ? parts.Skip(1).ToArray() : parts.Skip(1).SkipLast(1).ToArray();
        string prefix = hasSpace ? "" : currentWord;

        return command.GetParameterCompletions(parameters)
            .Where(c => c.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            .OrderBy(c => c);
    }
}
