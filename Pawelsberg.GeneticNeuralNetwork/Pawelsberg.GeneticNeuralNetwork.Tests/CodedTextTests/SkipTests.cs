using System;
using Pawelsberg.GeneticNeuralNetwork.Model;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.CodedTextTests;

public class SkipTests
{
    [Fact]
    public void AdvancesWhenTextMatches()
    {
        var codedText = new CodedText("hello world");

        codedText.Skip("hello");

        Assert.Equal(5, codedText.Index);
    }

    [Fact]
    public void ThrowsWhenTextDoesNotMatch()
    {
        var codedText = new CodedText("hello world");

        var exception = Assert.Throws<FormatException>(() => codedText.Skip("world"));
        Assert.Contains("Expected world", exception.Message);
    }

    [Fact]
    public void ThrowsAtEndOfText()
    {
        var codedText = new CodedText("hello");
        codedText.Index = 5;

        Assert.Throws<FormatException>(() => codedText.Skip("x"));
    }

    [Fact]
    public void IncludesIndexInExceptionMessage()
    {
        var codedText = new CodedText("hello world");
        codedText.Index = 6;

        var exception = Assert.Throws<FormatException>(() => codedText.Skip("hello"));
        Assert.Contains("6", exception.Message);
    }
}
