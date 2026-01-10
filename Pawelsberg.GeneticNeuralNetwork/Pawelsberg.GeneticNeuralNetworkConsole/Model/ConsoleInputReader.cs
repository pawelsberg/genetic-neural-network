namespace Pawelsberg.GeneticNeuralNetworkConsole.Model;

public class ConsoleInputReader
{
    private readonly ConsoleInputCompletions _completions;
    private string? _inputBeforeCompletion;

    private readonly ConsoleInputHistory _history;
    private string? _inputBeforeHistory;

    private List<char> _input;
    private int _cursorPosition;

    public ConsoleInputReader()
    {
        _completions = new ConsoleInputCompletions();
        _inputBeforeCompletion = null;

        _history = new ConsoleInputHistory();
        _inputBeforeHistory = null;

        _input = new List<char>();
        _cursorPosition = 0;
    }

    public string ReadLine()
    {
        _input = new List<char>();
        _cursorPosition = 0;
        ResetCompletion();
        _history.Reset();

        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);

            switch (keyInfo.Key)
            {
                case ConsoleKey.Enter:
                    Console.WriteLine();
                    string result = new string(_input.ToArray());
                    _history.AddEntry(result);
                    return result;

                case ConsoleKey.Backspace:
                    if (_cursorPosition > 0)
                    {
                        _input.RemoveAt(_cursorPosition - 1);
                        _cursorPosition--;
                        ResetCompletion();
                        _history.Reset();
                        RedrawLine();
                    }
                    break;

                case ConsoleKey.Delete:
                    if (_cursorPosition < _input.Count)
                    {
                        _input.RemoveAt(_cursorPosition);
                        ResetCompletion();
                        _history.Reset();
                        RedrawLine();
                    }
                    break;

                case ConsoleKey.LeftArrow:
                    if (_cursorPosition > 0)
                    {
                        _cursorPosition--;
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    }
                    ResetCompletion();
                    break;

                case ConsoleKey.RightArrow:
                    if (_cursorPosition < _input.Count)
                    {
                        _cursorPosition++;
                        Console.SetCursorPosition(Console.CursorLeft + 1, Console.CursorTop);
                    }
                    ResetCompletion();
                    break;

                case ConsoleKey.UpArrow:
                    HandleHistoryUp();
                    break;

                case ConsoleKey.DownArrow:
                    HandleHistoryDown();
                    break;

                case ConsoleKey.Home:
                    Console.SetCursorPosition(Console.CursorLeft - _cursorPosition, Console.CursorTop);
                    _cursorPosition = 0;
                    ResetCompletion();
                    break;

                case ConsoleKey.End:
                    Console.SetCursorPosition(Console.CursorLeft + (_input.Count - _cursorPosition), Console.CursorTop);
                    _cursorPosition = _input.Count;
                    ResetCompletion();
                    break;

                case ConsoleKey.Tab:
                    HandleTab(keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift));
                    break;

                case ConsoleKey.Escape:
                    // Clear the line
                    _input.Clear();
                    _cursorPosition = 0;
                    ResetCompletion();
                    _history.Reset();
                    RedrawLine();
                    break;

                default:
                    if (!char.IsControl(keyInfo.KeyChar))
                    {
                        _input.Insert(_cursorPosition, keyInfo.KeyChar);
                        _cursorPosition++;
                        ResetCompletion();
                        _history.Reset();
                        RedrawLine();
                    }
                    break;
            }
        }
    }

    private void HandleTab(bool shiftPressed)
    {
        _history.Reset();
        string currentInput = new string(_input.ToArray());

        // First TAB press - get completions
        if (!_completions.IsCycling)
        {
            _inputBeforeCompletion = currentInput;
            _completions.LoadCompletions(currentInput);

            if (_completions.CurrentCompletions.Count == 0)
                return;

            if (_completions.CurrentCompletions.Count == 1)
            {
                // Single completion - apply it directly
                ApplyCompletion(_completions.CurrentCompletions[0]);
                ResetCompletion();
                return;
            }

            // Multiple completions - show them and apply first
            ShowCompletions();
            string? completion = _completions.TryCycleNext();
            if (completion != null)
                ApplyCompletion(completion);
        }
        else
        {
            // Subsequent TAB press - cycle through completions
            string? completion = shiftPressed ? _completions.TryCyclePrevious() : _completions.TryCycleNext();
            if (completion != null)
                ApplyCompletion(completion);
        }
    }

    private void ApplyCompletion(string completion)
    {
        if (_inputBeforeCompletion == null)
            return;

        // Find what part to replace
        string[] parts = _inputBeforeCompletion.TrimStart().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        bool endsWithSpace = _inputBeforeCompletion.EndsWith(" ");

        string newInput;
        if (parts.Length == 0 || (parts.Length == 1 && !endsWithSpace))
        {
            // Completing command name
            newInput = completion;
        }
        else
        {
            // Completing a parameter
            string prefix = _inputBeforeCompletion;
            if (!endsWithSpace && parts.Length > 0)
            {
                // Remove the last partial word
                int lastSpaceIndex = _inputBeforeCompletion.LastIndexOf(' ');
                prefix = lastSpaceIndex >= 0 ? _inputBeforeCompletion.Substring(0, lastSpaceIndex + 1) : "";
            }
            newInput = prefix + completion;
        }

        // Update the input
        _input.Clear();
        _input.AddRange(newInput.ToCharArray());
        _cursorPosition = _input.Count;

        RedrawLine();
    }

    private void ShowCompletions()
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;

        int maxWidth = _completions.CurrentCompletions.Max(c => c.Length) + 2;
        int columns = Math.Max(1, Console.WindowWidth / maxWidth);
        int count = 0;

        foreach (string completion in _completions.CurrentCompletions)
        {
            Console.Write(completion.PadRight(maxWidth));
            count++;
            if (count % columns == 0)
                Console.WriteLine();
        }

        if (count % columns != 0)
            Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write(">");
    }

    private void ResetCompletion()
    {
        _completions.Reset();
        _inputBeforeCompletion = null;
    }

    private void RedrawLine()
    {
        // Calculate the starting position (after the prompt)
        int promptLength = 1; // ">"
        int startLeft = promptLength;

        // Save current cursor position
        int currentTop = Console.CursorTop;

        // Move to start of input area
        Console.SetCursorPosition(startLeft, currentTop);

        // Write the input
        string inputStr = new string(_input.ToArray());
        Console.Write(inputStr);

        // Clear any remaining characters from previous input
        int clearLength = Console.WindowWidth - startLeft - inputStr.Length - 1;
        if (clearLength > 0)
            Console.Write(new string(' ', clearLength));

        // Position cursor at the correct location
        Console.SetCursorPosition(startLeft + _cursorPosition, currentTop);
    }

    private void HandleHistoryUp()
    {
        if (!_history.IsNavigating)
        {
            _inputBeforeHistory = new string(_input.ToArray());
        }
        string? historyEntry = _history.TryNavigateUp();
        if (historyEntry != null)
            SetInputFromHistory(historyEntry);
    }

    private void HandleHistoryDown()
    {
        string? historyEntry = _history.TryNavigateDown();
        if (historyEntry != null)
        {
            SetInputFromHistory(historyEntry);
        }
        else if (!_history.IsNavigating && _inputBeforeHistory != null)
        {
            SetInputFromHistory(_inputBeforeHistory);
            _inputBeforeHistory = null;
        }
    }

    private void SetInputFromHistory(string historyEntry)
    {
        _input.Clear();
        _input.AddRange(historyEntry.ToCharArray());
        _cursorPosition = _input.Count;
        ResetCompletion();
        RedrawLine();
    }
}
