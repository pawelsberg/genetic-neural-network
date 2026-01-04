using System.Collections.Generic;
using Pawelsberg.GeneticNeuralNetwork.Model;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.CodedTextTests;

public class CodedLinesTests
{
    [Fact]
    public void ConstructorFromStringSplitsLines()
    {
        var lines = new CodedLines("line1\nline2\nline3");

        Assert.Equal(3, lines.Count);
        Assert.Equal("line1", lines[0]);
        Assert.Equal("line2", lines[1]);
        Assert.Equal("line3", lines[2]);
    }

    [Fact]
    public void ConstructorFromArrayStoresLines()
    {
        var lines = new CodedLines(new[] { "a", "b", "c" });

        Assert.Equal(3, lines.Count);
        Assert.Equal("a", lines[0]);
    }

    [Fact]
    public void IndexStartsAtZero()
    {
        var lines = new CodedLines("line1\nline2");

        Assert.Equal(0, lines.Index);
    }

    [Fact]
    public void EndOfLinesIsFalseAtStart()
    {
        var lines = new CodedLines("line1\nline2");

        Assert.False(lines.EndOfLines);
    }

    [Fact]
    public void EndOfLinesIsTrueAfterLastLine()
    {
        var lines = new CodedLines("line1");
        lines.Index = 1;

        Assert.True(lines.EndOfLines);
    }

    [Fact]
    public void CurrentLineReturnsCurrentLine()
    {
        var lines = new CodedLines("line1\nline2");

        Assert.Equal("line1", lines.CurrentLine);
        lines.Index = 1;
        Assert.Equal("line2", lines.CurrentLine);
    }

    [Fact]
    public void CurrentLineReturnsNullAtEnd()
    {
        var lines = new CodedLines("line1");
        lines.Index = 1;

        Assert.Null(lines.CurrentLine);
    }

    [Fact]
    public void CurrentContentReturnsTrimmedLine()
    {
        var lines = new CodedLines("  trimmed  ");

        Assert.Equal("trimmed", lines.CurrentContent);
    }

    [Fact]
    public void CurrentIndentReturnsIndentLevel()
    {
        var lines = new CodedLines("    indented");

        Assert.Equal(2, lines.CurrentIndent);
    }

    [Fact]
    public void AdvanceIncrementsIndex()
    {
        var lines = new CodedLines("line1\nline2");

        lines.Advance();

        Assert.Equal(1, lines.Index);
    }

    [Fact]
    public void ReadLineReturnsCurrentAndAdvances()
    {
        var lines = new CodedLines("line1\nline2");

        string? line = lines.ReadLine();

        Assert.Equal("line1", line);
        Assert.Equal(1, lines.Index);
    }

    [Fact]
    public void ReadLineReturnsNullAtEnd()
    {
        var lines = new CodedLines("line1");
        lines.Index = 1;

        string? line = lines.ReadLine();

        Assert.Null(line);
    }

    [Fact]
    public void GetIndentAtReturnsIndentForIndex()
    {
        var lines = new CodedLines("no indent\n  two spaces\n    four spaces");

        Assert.Equal(0, lines.GetIndentAt(0));
        Assert.Equal(1, lines.GetIndentAt(1));
        Assert.Equal(2, lines.GetIndentAt(2));
    }

    [Fact]
    public void GetIndentAtReturnsZeroForOutOfRange()
    {
        var lines = new CodedLines("line1");

        Assert.Equal(0, lines.GetIndentAt(10));
    }

    [Fact]
    public void CollectIndentedContentCollectsDeepLines()
    {
        var lines = new CodedLines("base\n  child1\n  child2\nsibling");
        lines.Index = 1; // Start at first child

        List<string> collected = lines.CollectIndentedContent(0);

        Assert.Equal(new[] { "child1", "child2" }, collected);
        Assert.Equal(3, lines.Index); // Stopped at "sibling"
    }

    [Fact]
    public void CollectIndentedContentReturnsEmptyWhenNoIndentedLines()
    {
        var lines = new CodedLines("line1\nline2");
        lines.Index = 1;

        List<string> collected = lines.CollectIndentedContent(0);

        Assert.Empty(collected);
    }

    [Fact]
    public void SkipEmptyLinesSkipsWhitespaceOnlyLines()
    {
        var lines = new CodedLines(new[] { "", "   ", "content" });

        lines.SkipEmptyLines();

        Assert.Equal(2, lines.Index);
        Assert.Equal("content", lines.CurrentLine);
    }

    [Fact]
    public void SkipEmptyLinesDoesNothingOnContent()
    {
        var lines = new CodedLines("content\nempty");

        lines.SkipEmptyLines();

        Assert.Equal(0, lines.Index);
    }

    [Fact]
    public void ToArrayReturnsUnderlyingArray()
    {
        var source = new[] { "a", "b", "c" };
        var lines = new CodedLines(source);

        string[] result = lines.ToArray();

        Assert.Equal(source, result);
    }
}
