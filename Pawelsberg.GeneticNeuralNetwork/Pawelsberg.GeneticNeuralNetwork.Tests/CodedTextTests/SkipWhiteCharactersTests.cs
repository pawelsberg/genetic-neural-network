using Pawelsberg.GeneticNeuralNetwork.Model;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.CodedTextTests;

public class SkipWhiteCharactersTests
{
    [Fact]
    public void SkipsSpaces()
    {
        var codedText = new CodedText("   text");

        codedText.SkipWhiteCharacters();

        Assert.Equal(3, codedText.Index);
        Assert.True(codedText.CanRead("text"));
    }

    [Fact]
    public void SkipsTabs()
    {
        var codedText = new CodedText("\t\ttext");

        codedText.SkipWhiteCharacters();

        Assert.Equal(2, codedText.Index);
        Assert.True(codedText.CanRead("text"));
    }

    [Fact]
    public void SkipsNewlines()
    {
        var codedText = new CodedText("\n\r\ntext");

        codedText.SkipWhiteCharacters();

        Assert.Equal(3, codedText.Index);
        Assert.True(codedText.CanRead("text"));
    }

    [Fact]
    public void SkipsMixedWhitespace()
    {
        var codedText = new CodedText(" \t\n\r text");

        codedText.SkipWhiteCharacters();

        Assert.Equal(5, codedText.Index);
        Assert.True(codedText.CanRead("text"));
    }

    [Fact]
    public void DoesNothingWhenNoWhitespace()
    {
        var codedText = new CodedText("text");

        codedText.SkipWhiteCharacters();

        Assert.Equal(0, codedText.Index);
    }

    [Fact]
    public void HandlesAllWhitespace()
    {
        var codedText = new CodedText("   ");

        codedText.SkipWhiteCharacters();

        Assert.Equal(3, codedText.Index);
        Assert.True(codedText.EOT);
    }
}
