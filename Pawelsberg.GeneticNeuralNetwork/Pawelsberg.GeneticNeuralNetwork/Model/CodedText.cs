namespace Pawelsberg.GeneticNeuralNetwork.Model;

public class CodedText
{
    private readonly char[] WiteChars = new[] { ' ', '\n', '\r', '\t' };
    private string Text { get; set; }
    public int Index { get; set; }
    public bool EOT { get { return Index >= Text.Length; } }
    public CodedText(string text) { Text = text; }
    public void SkipWhiteCharacters()
    {
        while (Index < Text.Length && WiteChars.Contains(Text[Index])) Index++;
    }

    public bool CanRead(string text)
    {
        return !EOT && Text.Substring(Index).StartsWith(text);
    }

    public bool TrySkip(string text)
    {
        if (CanRead(text))
        {
            Index += text.Length;
            return true;
        }
        return false;
    }
    public void Skip(string text)
    {
        if (!CanRead(text))
            throw new FormatException(string.Format("Expected {0} (character number {1})", text, Index));

        Index += text.Length;
    }

    public int ReadInt()
    {
        int intLength = NonWhiteCharacterFieldLength();
        int result = int.Parse(Text.Substring(Index, intLength));
        Index += intLength;
        return result;
    }

    public double ReadDouble()
    {
        int doubleLength = NonWhiteCharacterFieldLength();
        double result = double.Parse(Text.Substring(Index, doubleLength));
        Index += doubleLength;
        return result;
    }
    public string ReadString()
    {
        int stringLength = NonWhiteCharacterFieldLength();
        string result = Text.Substring(Index, stringLength);
        Index += stringLength;
        return result;
    }
    public string ReadString(int maxLength)
    {
        int stringLength = Math.Min(NonWhiteCharacterFieldLength(), maxLength);
        string result = Text.Substring(Index, stringLength);
        Index += stringLength;
        return result;
    }

    public string ReadParenthesesContent()
    {
        if (!TrySkip("("))
            throw new FormatException($"Expected '(' (character number {Index})");

        int start = Index;
        int closingIndex = FindMatchingParenthesis(Text, start - 1);
        string content = Text.Substring(start, closingIndex - start);
        Index = closingIndex + 1;
        return content;
    }

    public List<string> SplitTopLevel(string text)
    {
        List<string> parts = new List<string>();
        foreach ((int start, int length) in EnumerateTopLevelSegments(text, ','))
        {
            parts.Add(text.Substring(start, length));
        }
        return parts;
    }

    private int NonWhiteCharacterFieldLength()
    {
        int index = Index;

        while (index < Text.Length && !WiteChars.Contains(Text[index]))
        {
            index++;
        }
        return index - Index;
    }

    private static IEnumerable<(int start, int length)> EnumerateTopLevelSegments(string text, char separator)
    {
        int depth = 0;
        int start = 0;

        for (int i = 0; i < text.Length; i++)
        {
            AdjustDepth(text[i], ref depth);

            if (text[i] == separator && depth == 0)
            {
                yield return (start, i - start);
                start = i + 1;
            }
        }

        yield return (start, text.Length - start);
    }

    private static int FindMatchingParenthesis(string text, int openIndex)
    {
        int depth = 0;

        for (int i = openIndex; i < text.Length; i++)
        {
            AdjustDepth(text[i], ref depth);

            if (depth == 0)
                return i;
        }

        throw new FormatException($"Missing closing ')' for '(' at position {openIndex}");
    }

    // TODO - remove ref usage
    private static void AdjustDepth(char character, ref int depth)
    {
        if (character == '(') depth++;
        else if (character == ')') depth--;
    }

    /// <summary>
    /// Gets the indentation level of a line (number of leading spaces divided by 2).
    /// </summary>
    public static int GetIndentLevel(string line)
    {
        int spaces = 0;
        foreach (char c in line)
        {
            if (c == ' ') spaces++;
            else break;
        }
        return spaces / 2;
    }

    /// <summary>
    /// Extracts the text name (identifier) before the first parenthesis.
    /// </summary>
    public static string ExtractTextName(string content)
    {
        int parenIndex = content.IndexOf('(');
        if (parenIndex > 0)
        {
            return content.Substring(0, parenIndex).Trim();
        }
        return content.Trim();
    }

    /// <summary>
    /// Splits a text into lines, removing empty entries.
    /// </summary>
    public static string[] SplitIntoLines(string text)
    {
        return text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
    }

    /// <summary>
    /// Splits a hyphen-separated range string into two integers.
    /// Returns (value, value) if no hyphen is present.
    /// </summary>
    public static (int from, int to) SplitRange(string inner)
    {
        if (inner.Contains('-'))
        {
            int hyphenIndex = inner.IndexOf('-');
            int from = int.Parse(inner.Substring(0, hyphenIndex).Trim(), System.Globalization.CultureInfo.InvariantCulture);
            int to = int.Parse(inner.Substring(hyphenIndex + 1).Trim(), System.Globalization.CultureInfo.InvariantCulture);
            return (from, to);
        }
        else
        {
            int value = int.Parse(inner.Trim(), System.Globalization.CultureInfo.InvariantCulture);
            return (value, value);
        }
    }

    /// <summary>
    /// Splits a comma-separated parameter string into trimmed parts.
    /// </summary>
    public static string[] SplitParams(string inner)
    {
        return inner.Split(',').Select(s => s.Trim()).ToArray();
    }
}

/// <summary>
/// A wrapper for parsing multi-line text with indentation support.
/// </summary>
public class CodedLines
{
    private readonly string[] _lines;
    public int Index { get; set; }
    public int Count => _lines.Length;
    public bool EndOfLines => Index >= _lines.Length;

    public CodedLines(string text)
    {
        _lines = CodedText.SplitIntoLines(text);
    }

    public CodedLines(string[] lines)
    {
        _lines = lines;
    }

    /// <summary>
    /// Gets a line by absolute index without changing the current index.
    /// </summary>
    public string this[int index] => _lines[index];

    /// <summary>
    /// Gets the current line or null if at end.
    /// </summary>
    public string? CurrentLine => Index < _lines.Length ? _lines[Index] : null;

    /// <summary>
    /// Gets the current line's trimmed content or null if at end.
    /// </summary>
    public string? CurrentContent => CurrentLine?.Trim();

    /// <summary>
    /// Gets the current line's indent level.
    /// </summary>
    public int CurrentIndent => CurrentLine != null ? CodedText.GetIndentLevel(CurrentLine) : 0;

    /// <summary>
    /// Advances to the next line.
    /// </summary>
    public void Advance() => Index++;

    /// <summary>
    /// Reads and advances the current line if it exists.
    /// </summary>
    public string? ReadLine()
    {
        if (EndOfLines) return null;
        return _lines[Index++];
    }

    /// <summary>
    /// Gets the indent level at a specific index.
    /// </summary>
    public int GetIndentAt(int index) => index < _lines.Length ? CodedText.GetIndentLevel(_lines[index]) : 0;

    /// <summary>
    /// Collects all lines at a deeper indent than the base indent, advancing the index.
    /// Returns the trimmed content of each collected line.
    /// </summary>
    public List<string> CollectIndentedContent(int baseIndent)
    {
        var specs = new List<string>();
        while (!EndOfLines)
        {
            int indent = CurrentIndent;
            if (indent <= baseIndent)
                break;
            specs.Add(CurrentContent!);
            Advance();
        }
        return specs;
    }

    /// <summary>
    /// Skips empty lines (blank or whitespace-only).
    /// </summary>
    public void SkipEmptyLines()
    {
        while (!EndOfLines && string.IsNullOrWhiteSpace(CurrentLine))
        {
            Advance();
        }
    }

    /// <summary>
    /// Gets the underlying lines array (for interop with legacy code).
    /// </summary>
    public string[] ToArray() => _lines;
}
