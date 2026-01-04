using System.Globalization;
using System.Text;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting.DiskStoring;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting.QualityMeasuring.DiskStoring;

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

        // Handle meters with propagations (TestCase meters) - they expand from TestCases placeholder
        if (meter is INetworkQualityMeterWithPropagations)
        {
            // Write the meter line
            if (meter is INetworkQualityMeterTextConvertible convertible)
            {
                sb.AppendLine($"{indent}{convertible.ToText()}");
            }

            // Write children
            foreach (var child in meter.Children)
            {
                WriteQualityMeter(sb, child, indentLevel + 1);
            }
        }
        else if (meter.Parent == null)
        {
            // Root meter - check if we should write TestCases block
            var testCaseMeters = meter.Children.Where(c => c is INetworkQualityMeterWithPropagations).Cast<INetworkQualityMeterWithPropagations>().ToList();
            var otherMeters = meter.Children.Where(c => !(c is INetworkQualityMeterWithPropagations)).ToList();

            if (testCaseMeters.Any())
            {
                // Detect propagation range
                var distinctProps = testCaseMeters.Select(m => m.Propagations).Distinct().OrderBy(p => p).ToList();
                int propsFrom = distinctProps.First();
                int propsTo = distinctProps.Last();

                // Write TestCases with range if needed
                if (propsFrom == propsTo)
                {
                    sb.AppendLine($"{indent}{QualityMeterTypeRegistry.TestCasesPlaceholderTextName}({propsFrom.ToString(CultureInfo.InvariantCulture)})");
                }
                else
                {
                    sb.AppendLine($"{indent}{QualityMeterTypeRegistry.TestCasesPlaceholderTextName}({propsFrom.ToString(CultureInfo.InvariantCulture)}-{propsTo.ToString(CultureInfo.InvariantCulture)})");
                }

                // Write children template from first test case meter
                var templateMeter = (QualityMeter<Network>)testCaseMeters.First();
                foreach (var child in templateMeter.Children)
                {
                    WriteQualityMeter(sb, child, indentLevel + 1);
                }
            }

            // Write non-test-case children
            foreach (var child in otherMeters)
            {
                WriteQualityMeter(sb, child, indentLevel);
            }
        }
        else
        {
            // Normal meter
            if (meter is INetworkQualityMeterTextConvertible convertible)
            {
                sb.AppendLine($"{indent}{convertible.ToText()}");
            }

            // Write children
            foreach (var child in meter.Children)
            {
                WriteQualityMeter(sb, child, indentLevel + 1);
            }
        }
    }

    public static QualityMeter<Network> Parse(string text, int propagations, TestCaseList testCaseList)
    {
        CodedLines lines = new CodedLines(text);
        
        if (lines.Count == 0)
            return new QualityMeter<Network>(null);

        // Check if first line is a container type
        string firstLine = lines[0].Trim();
        
        // Try container types first (TestCasesSequential, TestCases)
        var containerDescriptor = QualityMeterTypeRegistry.GetContainerDescriptorForLine(firstLine);
        if (containerDescriptor != null)
        {
            return containerDescriptor.ContainerParser(lines, testCaseList, ParseSingleMeter);
        }
        
        // Try single-line meter types that can be root (e.g., TestCaseList/Aggregate)
        string textName = CodedText.ExtractTextName(firstLine);
        if (!string.IsNullOrEmpty(textName) && QualityMeterTypeRegistry.ByTextName.ContainsKey(textName))
        {
            CodedText codedText = new CodedText(firstLine);
            codedText.TrySkip(textName);
            string inner = codedText.ReadParenthesesContent();
            var rootMeter = QualityMeterTypeRegistry.TryParse(textName, inner, null, propagations, testCaseList);
            if (rootMeter != null)
            {
                return rootMeter;
            }
        }

        // Fallback to static parsing for other formats
        QualityMeter<Network> rootMeter2 = new QualityMeter<Network>(null);
        ParseMetersStatic(lines, rootMeter2, propagations, testCaseList);
        return rootMeter2;
    }

    private static void ParseMetersStatic(CodedLines lines, QualityMeter<Network> parent, int propagations, TestCaseList testCaseList)
    {
        int parentIndent = parent.Parent == null ? -1 : (lines.Index > 0 ? lines.GetIndentAt(lines.Index - 1) : 0);

        while (!lines.EndOfLines)
        {
            int indent = lines.CurrentIndent;
            string content = lines.CurrentContent!;

            if (string.IsNullOrEmpty(content))
            {
                lines.Advance();
                continue;
            }

            // Check if we've gone back to a lower indent level
            if (indent <= parentIndent && parent.Parent != null)
            {
                return;
            }

            // Check if this is a placeholder type
            string textName = CodedText.ExtractTextName(content);
            if (QualityMeterTypeRegistry.IsPlaceholderType(textName))
            {
                CodedText codedText = new CodedText(content);
                codedText.TrySkip(textName);
                string parameters = codedText.ReadParenthesesContent();
                var placeholder = QualityMeterTypeRegistry.TryParsePlaceholder(textName, parameters);
                
                if (placeholder != null)
                {
                    lines.Advance();

                    // Collect child meter specs
                    var childSpecs = lines.CollectIndentedContent(indent);

                    // Expand placeholder using registry
                    QualityMeterTypeRegistry.ExpandPlaceholder(placeholder, parent, childSpecs, propagations, testCaseList, ParseSingleMeter);
                    continue;
                }
            }

            // Parse as regular meter
            var meter = ParseSingleMeter(content, parent, propagations, testCaseList);

            if (meter != null)
            {
                parent.Children.Add(meter);
                lines.Advance();

                // Parse children
                if (!lines.EndOfLines && lines.CurrentIndent > indent)
                {
                    ParseMetersStatic(lines, meter, propagations, testCaseList);
                }
            }
            else
            {
                lines.Advance();
            }
        }
    }

    private static QualityMeter<Network>? ParseSingleMeter(string content, QualityMeter<Network> parent, int propagations, TestCaseList testCaseList)
    {
        CodedText codedText = new CodedText(content);
        codedText.SkipWhiteCharacters();

        // Try to parse using the registry for all meter types
        string textName = CodedText.ExtractTextName(content);
        if (!string.IsNullOrEmpty(textName))
        {
            codedText.TrySkip(textName);
            string parameters = codedText.ReadParenthesesContent();
            var meter = QualityMeterTypeRegistry.TryParse(textName, parameters, parent, propagations, testCaseList);
            if (meter != null)
            {
                return meter;
            }
        }

        return null;
    }
}
