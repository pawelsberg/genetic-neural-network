namespace Pawelsberg.GeneticNeuralNetwork.Model
{
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
        private int NonWhiteCharacterFieldLength()
        {
            int index = Index;

            while (index < Text.Length && !WiteChars.Contains(Text[index]))
            {
                index++;
            }
            return index - Index;
        }
    }
}
