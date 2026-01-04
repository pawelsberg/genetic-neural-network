using Pawelsberg.GeneticNeuralNetwork.Model;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.CodedTextTests;

public class CanReadTests
{
    [Fact]
    public void ReturnsTrueWhenTextMatches()
    {
        var codedText = new CodedText("hello world");

        bool canRead = codedText.CanRead("hello");

        Assert.True(canRead);
    }

    [Fact]
    public void ReturnsFalseWhenTextDoesNotMatch()
    {
        var codedText = new CodedText("hello world");

        bool canRead = codedText.CanRead("world");

        Assert.False(canRead);
    }

    [Fact]
    public void ReturnsTrueAfterIndexAdvanced()
    {
        var codedText = new CodedText("hello world");
        codedText.Index = 6;

        bool canRead = codedText.CanRead("world");

        Assert.True(canRead);
    }

    [Fact]
    public void ReturnsFalseAtEndOfText()
    {
        var codedText = new CodedText("hello");
        codedText.Index = 5;

        bool canRead = codedText.CanRead("x");

        Assert.False(canRead);
    }

    [Fact]
    public void ReturnsTrueForEmptyString()
    {
        var codedText = new CodedText("hello");

        bool canRead = codedText.CanRead("");

        Assert.True(canRead);
    }

    [Fact]
    public void ReturnsTrueForExactMatch()
    {
        var codedText = new CodedText("hello");

        bool canRead = codedText.CanRead("hello");

        Assert.True(canRead);
    }
}
