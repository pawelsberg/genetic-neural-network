using System;
using Pawelsberg.GeneticNeuralNetwork.Model;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.CodedTextTests;

public class SplitRangeTests
{
    [Fact]
    public void ParsesRangeWithHyphen()
    {
        var (from, to) = CodedText.SplitRange("1-5");

        Assert.Equal(1, from);
        Assert.Equal(5, to);
    }

    [Fact]
    public void ParsesSingleValueAsRange()
    {
        var (from, to) = CodedText.SplitRange("10");

        Assert.Equal(10, from);
        Assert.Equal(10, to);
    }

    [Fact]
    public void TrimsWhitespace()
    {
        var (from, to) = CodedText.SplitRange("  3 - 7  ");

        Assert.Equal(3, from);
        Assert.Equal(7, to);
    }

    [Fact]
    public void TrimsWhitespaceForSingleValue()
    {
        var (from, to) = CodedText.SplitRange("  42  ");

        Assert.Equal(42, from);
        Assert.Equal(42, to);
    }

    [Fact]
    public void ParsesZero()
    {
        var (from, to) = CodedText.SplitRange("0");

        Assert.Equal(0, from);
        Assert.Equal(0, to);
    }

    [Fact]
    public void ParsesZeroRange()
    {
        var (from, to) = CodedText.SplitRange("0-0");

        Assert.Equal(0, from);
        Assert.Equal(0, to);
    }

    [Fact]
    public void ThrowsOnInvalidFormat()
    {
        Assert.Throws<FormatException>(() => CodedText.SplitRange("abc"));
    }

    [Fact]
    public void ThrowsOnEmptyString()
    {
        Assert.Throws<FormatException>(() => CodedText.SplitRange(""));
    }
}
