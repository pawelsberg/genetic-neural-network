using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring.DiskStoring;

public static class NetworkQualityMeterSectionExtensions
{
    public const string PerTestCaseSection = "PerTestCase:";
    public const string FromTestCasesSection = "FromTestCases:";
    public const string FromNetworkSection = "FromNetwork:";

    // Priority order: PerTestCase wins over FromTestCases wins over FromNetwork. A meter
    // implementing both IFromTestCases and IFromNetwork (e.g. IfAllGood) is reported as
    // FromTestCases — its primary section for serialization. Order here is load-bearing.
    public static string? GetSectionHeader(this QualityMeter<Network> meter) => meter switch
    {
        IPerTestCaseNetworkQualityMeter => PerTestCaseSection,
        IFromTestCasesNetworkQualityMeter => FromTestCasesSection,
        IFromNetworkNetworkQualityMeter => FromNetworkSection,
        _ => null
    };

    public static bool BelongsInSection(this QualityMeter<Network> meter, string sectionHeader) =>
        sectionHeader switch
        {
            PerTestCaseSection => meter is IPerTestCaseNetworkQualityMeter,
            FromTestCasesSection => meter is IFromTestCasesNetworkQualityMeter,
            FromNetworkSection => meter is IFromNetworkNetworkQualityMeter,
            _ => false
        };

    public static void ValidateInSection(this QualityMeter<Network> meter, string sectionHeader)
    {
        if (!meter.BelongsInSection(sectionHeader))
            throw new InvalidOperationException(
                $"Meter '{meter.GetType().Name}' is not allowed in '{sectionHeader}' section. Move it to the correct section.");
    }
}
