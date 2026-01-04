using System;
using Pawelsberg.GeneticNeuralNetwork.Model;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.CodedTextTests;

public class ReadIntTests
{
    [Fact]
    public void ReadsPositiveInteger()
    {
        var codedText = new CodedText("42");

        int value = codedText.ReadInt();

        Assert.Equal(42, value);
    }

    [Fact]
    public void ReadsNegativeInteger()
    {
        var codedText = new CodedText("-42");

        int value = codedText.ReadInt();

        Assert.Equal(-42, value);
    }

    [Fact]
    public void AdvancesIndexAfterRead()
    {
        var codedText = new CodedText("42 rest");

        codedText.ReadInt();

        Assert.Equal(2, codedText.Index);
    }

    [Fact]
    public void StopsAtWhitespace()
    {
        var codedText = new CodedText("123 456");

        int value = codedText.ReadInt();

        Assert.Equal(123, value);
        Assert.Equal(3, codedText.Index);
    }

    [Fact]
    public void ReadsZero()
    {
        var codedText = new CodedText("0");

        int value = codedText.ReadInt();

        Assert.Equal(0, value);
    }

    [Fact]
    public void ThrowsOnInvalidFormat()
    {
        var codedText = new CodedText("abc");

        Assert.Throws<FormatException>(() => codedText.ReadInt());
    }
}
