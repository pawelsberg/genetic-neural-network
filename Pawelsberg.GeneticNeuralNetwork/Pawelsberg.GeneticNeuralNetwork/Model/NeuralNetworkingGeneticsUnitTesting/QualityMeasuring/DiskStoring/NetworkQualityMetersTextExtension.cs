using System.Text;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring.DiskStoring;

public static class NetworkQualityMetersTextExtension
{
    public static string ToText(this QualityMeter<Network> meter)
    {
        StringBuilder sb = new StringBuilder();
        
        // Handle container types using the interface
        if (meter is INetworkQualityMeterContainerTextConvertible containerConvertible)
        {
            containerConvertible.WriteToText(sb);
            return sb.ToString();
        }
        
        // Handle TestCaseList meters
        if (meter is INetworkQualityMeterTextConvertible textConvertible && meter.Parent == null && meter.Children.Count == 0)
        {
            sb.AppendLine(textConvertible.ToText());
            return sb.ToString();
        }
        
        WriteQualityMeter(sb, meter, 0);
        return sb.ToString();
    }

    private static void WriteQualityMeter(StringBuilder sb, QualityMeter<Network> meter, int indentLevel)
    {
        string indent = new string(' ', indentLevel * 2);

        if (meter is INetworkQualityMeterTextConvertible convertible)
            sb.AppendLine($"{indent}{convertible.ToText()}");

        foreach (var child in meter.Children)
            WriteQualityMeter(sb, child, indentLevel + 1);
    }

    public static QualityMeter<Network> Parse(string text)
    {
        CodedText codedText = new CodedText(text);

        if (codedText.EOT)
            return new QualityMeter<Network>(null);

        // Check if first line is a container type
        string firstLine = codedText.CurrentLineContent!;

        // Try container types first (TestCasesSequential, TestCases)
        ContainerQualityMeterTypeDescriptor? containerDescriptor = QualityMeterTypeRegistry.GetContainerDescriptorForLine(firstLine);
        if (containerDescriptor != null)
            return containerDescriptor.ContainerParser(codedText, ParseSingleMeter);

        // Try single-line meter types that can be root (e.g., TestCaseList/Aggregate, SequentialOutputTestCase)
        codedText.SkipWhiteCharacters();
        if (codedText.TryReadName(out string textName))
        {
            string inner = codedText.ReadParenthesesContent();
            QualityMeter<Network>? rootMeter = QualityMeterTypeRegistry.TryParse(textName, inner, null);
            if (rootMeter != null)
                return rootMeter;
        }

        throw new InvalidOperationException($"Unknown quality meter format: {firstLine}");
    }

    private static QualityMeter<Network>? ParseSingleMeter(string content, QualityMeter<Network> parent)
    {
        CodedText codedText = new CodedText(content);
        codedText.SkipWhiteCharacters();

        if (!codedText.TryReadName(out string textName))
            return null;

        string parameters = codedText.ReadParenthesesContent();
        return QualityMeterTypeRegistry.TryParse(textName, parameters, parent);
    }
}
