using Pawelsberg.GeneticNeuralNetwork.Model;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.CodedTextTests;

public class ReadStringTests
{
    [Fact]
    public void ReadsUntilWhitespace()
    {
        var codedText = new CodedText("hello world");

        string value = codedText.ReadString();

        Assert.Equal("hello", value);
    }

    [Fact]
    public void AdvancesIndexAfterRead()
    {
        var codedText = new CodedText("hello world");

        codedText.ReadString();

        Assert.Equal(5, codedText.Index);
    }

    [Fact]
    public void ReadsEntireStringWhenNoWhitespace()
    {
        var codedText = new CodedText("hello");

        string value = codedText.ReadString();

        Assert.Equal("hello", value);
    }

    [Fact]
    public void ReadsEmptyStringAtEndOfText()
    {
        var codedText = new CodedText("hello");
        codedText.Index = 5;

        string value = codedText.ReadString();

        Assert.Equal("", value);
    }

    [Fact]
    public void ReadsWithMaxLength()
    {
        var codedText = new CodedText("hello world");

        string value = codedText.ReadString(3);

        Assert.Equal("hel", value);
        Assert.Equal(3, codedText.Index);
    }

    [Fact]
    public void MaxLengthDoesNotExceedField()
    {
        var codedText = new CodedText("hi world");

        string value = codedText.ReadString(10);

        Assert.Equal("hi", value);
        Assert.Equal(2, codedText.Index);
    }
}
