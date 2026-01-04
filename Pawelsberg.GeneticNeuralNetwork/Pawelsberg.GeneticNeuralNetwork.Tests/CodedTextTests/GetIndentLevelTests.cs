using Pawelsberg.GeneticNeuralNetwork.Model;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.CodedTextTests;

public class GetIndentLevelTests
{
    [Fact]
    public void ReturnsZeroForNoIndent()
    {
        int level = CodedText.GetIndentLevel("text");

        Assert.Equal(0, level);
    }

    [Fact]
    public void ReturnsTwoSpacesAsLevelOne()
    {
        int level = CodedText.GetIndentLevel("  text");

        Assert.Equal(1, level);
    }

    [Fact]
    public void ReturnsFourSpacesAsLevelTwo()
    {
        int level = CodedText.GetIndentLevel("    text");

        Assert.Equal(2, level);
    }

    [Fact]
    public void ReturnsZeroForEmptyString()
    {
        int level = CodedText.GetIndentLevel("");

        Assert.Equal(0, level);
    }

    [Fact]
    public void ReturnsZeroForOddSingleSpace()
    {
        int level = CodedText.GetIndentLevel(" text");

        Assert.Equal(0, level);
    }

    [Fact]
    public void ReturnsOneForThreeSpaces()
    {
        int level = CodedText.GetIndentLevel("   text");

        Assert.Equal(1, level);
    }

    [Fact]
    public void HandlesOnlySpaces()
    {
        int level = CodedText.GetIndentLevel("      ");

        Assert.Equal(3, level);
    }
}
