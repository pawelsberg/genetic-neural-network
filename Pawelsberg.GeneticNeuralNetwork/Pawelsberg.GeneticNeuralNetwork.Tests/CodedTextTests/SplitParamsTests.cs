using Pawelsberg.GeneticNeuralNetwork.Model;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.CodedTextTests;

public class SplitParamsTests
{
    [Fact]
    public void SplitsOnComma()
    {
        string[] parts = CodedText.SplitParams("a,b,c");

        Assert.Equal(new[] { "a", "b", "c" }, parts);
    }

    [Fact]
    public void TrimsWhitespace()
    {
        string[] parts = CodedText.SplitParams("  a  ,  b  ,  c  ");

        Assert.Equal(new[] { "a", "b", "c" }, parts);
    }

    [Fact]
    public void ReturnsSinglePartForNoComma()
    {
        string[] parts = CodedText.SplitParams("single");

        Assert.Single(parts);
        Assert.Equal("single", parts[0]);
    }

    [Fact]
    public void HandlesEmptyParts()
    {
        string[] parts = CodedText.SplitParams("a,,b");

        Assert.Equal(new[] { "a", "", "b" }, parts);
    }

    [Fact]
    public void HandlesEmptyString()
    {
        string[] parts = CodedText.SplitParams("");

        Assert.Single(parts);
        Assert.Equal("", parts[0]);
    }

    [Fact]
    public void HandlesTwoParams()
    {
        string[] parts = CodedText.SplitParams("1.0, 0.001");

        Assert.Equal(new[] { "1.0", "0.001" }, parts);
    }
}
