namespace Pawelsberg.GeneticNeuralNetworkConsole.Model;

public class ConsoleInputHistory
{
    private readonly List<string> _history;
    private int? _historyIndex; // Current position in history during navigation; null when not navigating

    public ConsoleInputHistory()
    {
        _history = new List<string>();
        _historyIndex = null;
    }

    public bool IsNavigating => _historyIndex != null;

    public void AddEntry(string entry)
    {
        if (!string.IsNullOrWhiteSpace(entry) && (_history.Count == 0 || _history[^1] != entry))
        {
            _history.Add(entry);
        }
    }

    public string? TryNavigateUp()
    {
        if (_history.Count == 0)
            return null;

        if (_historyIndex == null)
            _historyIndex = _history.Count;

        if (_historyIndex > 0)
        {
            _historyIndex--;
            return _history[_historyIndex.Value];
        }

        return null;
    }

    public string? TryNavigateDown()
    {
        if (_historyIndex == null)
            return null;

        if (_historyIndex < _history.Count - 1)
        {
            _historyIndex++;
            return _history[_historyIndex.Value];
        }
        else if (_historyIndex == _history.Count - 1)
            _historyIndex = null;

        return null;
    }

    public void Reset()
    {
        _historyIndex = null;
    }
}
