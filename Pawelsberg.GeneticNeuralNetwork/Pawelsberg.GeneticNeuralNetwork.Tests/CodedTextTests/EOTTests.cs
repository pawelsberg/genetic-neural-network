using Pawelsberg.GeneticNeuralNetwork.Model;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.CodedTextTests;

public class EOTTests
{
    [Fact]
    public void ReturnsFalseAtStart()
    {
        var codedText = new CodedText("hello");

        Assert.False(codedText.EOT);
    }

    [Fact]
    public void ReturnsTrueAtEnd()
    {
        var codedText = new CodedText("hello");
        codedText.Index = 5;

        Assert.True(codedText.EOT);
    }

    [Fact]
    public void ReturnsTrueBeyondEnd()
    {
        var codedText = new CodedText("hello");
        codedText.Index = 10;

        Assert.True(codedText.EOT);
    }

    [Fact]
    public void ReturnsTrueForEmptyString()
    {
        var codedText = new CodedText("");

        Assert.True(codedText.EOT);
    }

    [Fact]
    public void ReturnsFalseOneBeforeEnd()
    {
        var codedText = new CodedText("hello");
        codedText.Index = 4;

        Assert.False(codedText.EOT);
    }
}
