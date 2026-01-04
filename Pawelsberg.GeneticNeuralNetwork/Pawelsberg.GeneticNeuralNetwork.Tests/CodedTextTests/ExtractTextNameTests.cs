using Pawelsberg.GeneticNeuralNetwork.Model;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.CodedTextTests;

public class ExtractTextNameTests
{
    [Fact]
    public void ExtractsNameBeforeParenthesis()
    {
        string name = CodedText.ExtractTextName("TestCases(10)");

        Assert.Equal("TestCases", name);
    }

    [Fact]
    public void ReturnsFullStringWhenNoParenthesis()
    {
        string name = CodedText.ExtractTextName("SomeText");

        Assert.Equal("SomeText", name);
    }

    [Fact]
    public void TrimsWhitespace()
    {
        string name = CodedText.ExtractTextName("  TestCases  (10)");

        Assert.Equal("TestCases", name);
    }

    [Fact]
    public void TrimsWhitespaceWhenNoParenthesis()
    {
        string name = CodedText.ExtractTextName("  SomeText  ");

        Assert.Equal("SomeText", name);
    }

    [Fact]
    public void ReturnsEmptyForEmptyString()
    {
        string name = CodedText.ExtractTextName("");

        Assert.Equal("", name);
    }

    [Fact]
    public void HandlesParenthesisAtStart()
    {
        string name = CodedText.ExtractTextName("(10)");

        Assert.Equal("(10)", name);
    }

    [Fact]
    public void HandlesNestedParentheses()
    {
        string name = CodedText.ExtractTextName("Func(a(b,c))");

        Assert.Equal("Func", name);
    }
}
