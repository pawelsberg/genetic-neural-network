using System;
using Pawelsberg.GeneticNeuralNetwork.Model;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.CodedTextTests;

public class ReadParenthesesContentTests
{
    [Fact]
    public void ReturnsInnerAndAdvancesIndex()
    {
        var codedText = new CodedText("(a(b,c),d)e");

        string content = codedText.ReadParenthesesContent();

        Assert.Equal("a(b,c),d", content);
        Assert.Equal(10, codedText.Index);
        Assert.True(codedText.CanRead("e"));
    }

    [Fact]
    public void ThrowsOnMissingClosingParenthesis()
    {
        var codedText = new CodedText("(abc");

        Assert.Throws<FormatException>(() => codedText.ReadParenthesesContent());
    }

    [Fact]
    public void ThrowsWhenNotStartingWithOpening()
    {
        var codedText = new CodedText("abc");

        var exception = Assert.Throws<FormatException>(() => codedText.ReadParenthesesContent());
        Assert.Contains("Expected '('", exception.Message);
    }
}
