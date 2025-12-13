using System.Collections.Generic;
using Pawelsberg.GeneticNeuralNetwork.Model;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.CodedTextTests;

public class SplitTopLevelTests
{
    [Fact]
    public void IgnoresNestedParentheses()
    {
        var codedText = new CodedText(string.Empty);

        List<string> parts = codedText.SplitTopLevel("a,(b,c(d,e)),f,g(h,(i,j))");

        Assert.Equal(new[] { "a", "(b,c(d,e))", "f", "g(h,(i,j))" }, parts);
    }

    [Fact]
    public void ReturnsWholeStringWhenNoSeparators()
    {
        var codedText = new CodedText(string.Empty);

        List<string> parts = codedText.SplitTopLevel("(a(b))");

        Assert.Single(parts);
        Assert.Equal("(a(b))", parts[0]);
    }
}
