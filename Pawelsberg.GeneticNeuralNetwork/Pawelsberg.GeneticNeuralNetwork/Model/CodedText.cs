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
}
