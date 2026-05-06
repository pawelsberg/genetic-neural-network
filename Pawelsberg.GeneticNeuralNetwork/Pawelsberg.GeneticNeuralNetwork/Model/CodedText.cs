namespace Pawelsberg.GeneticNeuralNetwork.Model;

public class CodedText
{
    private readonly char[] WiteChars = new[] { ' ', '\n', '\r', '\t' };
    private string Text { get; set; }
    public int Index { get; set; }
    public bool EOT { get { return Index >= Text.Length; } }
    public CodedText(string text)
    {
        Text = text;
        SkipBlankLines();
    }
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

    public static List<string> SplitTopLevel(string text)
    {
        List<string> parts = new List<string>();
        foreach ((int start, int length) in EnumerateTopLevelSegments(text, ','))
        {
            parts.Add(text.Substring(start, length));
        }
        return parts;
    }

    /// <summary>
    /// Trimmed content of the line containing Index — from Index forward to the next newline.
    /// Null if at end. Intended to be called when Index sits at the start of a line.
    /// </summary>
    public string? CurrentLineContent
    {
        get
        {
            if (EOT) return null;
            int end = Index;
            while (end < Text.Length && Text[end] != '\r' && Text[end] != '\n') end++;
            return Text.Substring(Index, end - Index).Trim();
        }
    }

    /// <summary>
    /// Indent level (leading-spaces / 2) measured from Index forward.
    /// Caller must be at line start for this to reflect the line's indent.
    /// </summary>
    public int CurrentIndent
    {
        get
        {
            int spaces = 0;
            while (Index + spaces < Text.Length && Text[Index + spaces] == ' ') spaces++;
            return spaces / 2;
        }
    }

    /// <summary>
    /// Advances Index past the rest of the current line and any trailing blank lines.
    /// After this call Index is at the start of the next non-blank line, or at end-of-text.
    /// </summary>
    public void AdvanceLine()
    {
        while (Index < Text.Length && Text[Index] != '\r' && Text[Index] != '\n') Index++;
        SkipBlankLines();
    }

    private void SkipBlankLines()
    {
        while (Index < Text.Length && (Text[Index] == '\r' || Text[Index] == '\n')) Index++;
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
            depth = AdjustDepth(text[i], depth);

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
            depth = AdjustDepth(text[i], depth);

            if (depth == 0)
                return i;
        }

        throw new FormatException($"Missing closing ')' for '(' at position {openIndex}");
    }

    private static int AdjustDepth(char character, int depth)
    {
        if (character == '(') depth++;
        else if (character == ')') depth--;
        return depth;
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
    /// Splits a comma-separated parameter string into trimmed parts.
    /// </summary>
    public static string[] SplitParams(string inner)
    {
        return inner.Split(',').Select(s => s.Trim()).ToArray();
    }
}
