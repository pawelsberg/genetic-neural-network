using Pawelsberg.GeneticNeuralNetwork.Model;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.CodedTextTests;

public class SplitIntoLinesTests
{
    [Fact]
    public void SplitsOnNewline()
    {
        string[] lines = CodedText.SplitIntoLines("line1\nline2\nline3");

        Assert.Equal(new[] { "line1", "line2", "line3" }, lines);
    }

    [Fact]
    public void SplitsOnCarriageReturn()
    {
        string[] lines = CodedText.SplitIntoLines("line1\rline2\rline3");

        Assert.Equal(new[] { "line1", "line2", "line3" }, lines);
    }

    [Fact]
    public void SplitsOnCarriageReturnNewline()
    {
        string[] lines = CodedText.SplitIntoLines("line1\r\nline2\r\nline3");

        Assert.Equal(new[] { "line1", "line2", "line3" }, lines);
    }

    [Fact]
    public void RemovesEmptyLines()
    {
        string[] lines = CodedText.SplitIntoLines("line1\n\nline2\n\n\nline3");

        Assert.Equal(new[] { "line1", "line2", "line3" }, lines);
    }

    [Fact]
    public void ReturnsEmptyArrayForEmptyString()
    {
        string[] lines = CodedText.SplitIntoLines("");

        Assert.Empty(lines);
    }

    [Fact]
    public void ReturnsSingleLineForNoNewlines()
    {
        string[] lines = CodedText.SplitIntoLines("single line");

        Assert.Single(lines);
        Assert.Equal("single line", lines[0]);
    }

    [Fact]
    public void PreservesIndentation()
    {
        string[] lines = CodedText.SplitIntoLines("  indented\n    more indented");

        Assert.Equal(new[] { "  indented", "    more indented" }, lines);
    }
}
