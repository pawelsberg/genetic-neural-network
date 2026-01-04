using Pawelsberg.GeneticNeuralNetwork.Model;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.CodedTextTests;

public class TrySkipTests
{
    [Fact]
    public void ReturnsTrueAndAdvancesWhenTextMatches()
    {
        var codedText = new CodedText("hello world");

        bool result = codedText.TrySkip("hello");

        Assert.True(result);
        Assert.Equal(5, codedText.Index);
    }

    [Fact]
    public void ReturnsFalseAndDoesNotAdvanceWhenTextDoesNotMatch()
    {
        var codedText = new CodedText("hello world");

        bool result = codedText.TrySkip("world");

        Assert.False(result);
        Assert.Equal(0, codedText.Index);
    }

    [Fact]
    public void WorksAfterPreviousSkip()
    {
        var codedText = new CodedText("hello world");
        codedText.TrySkip("hello ");

        bool result = codedText.TrySkip("world");

        Assert.True(result);
        Assert.Equal(11, codedText.Index);
    }

    [Fact]
    public void ReturnsTrueForEmptyString()
    {
        var codedText = new CodedText("hello");

        bool result = codedText.TrySkip("");

        Assert.True(result);
        Assert.Equal(0, codedText.Index);
    }

    [Fact]
    public void ReturnsFalseAtEndOfText()
    {
        var codedText = new CodedText("hello");
        codedText.Index = 5;

        bool result = codedText.TrySkip("x");

        Assert.False(result);
        Assert.Equal(5, codedText.Index);
    }
}
