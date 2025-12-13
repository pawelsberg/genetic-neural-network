using System.Globalization;
using Pawelsberg.GeneticNeuralNetwork.Model;
using Xunit;

namespace Pawelsberg.GeneticNeuralNetwork.Tests.CodedTextTests;

public class ReadDoubleTests
{
    [Fact]
    public void UsesCurrentCulture()
    {
        CultureInfo originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            var codedText = new CodedText("3.14");

            double value = codedText.ReadDouble();

            Assert.Equal(3.14, value, 3);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }
}
