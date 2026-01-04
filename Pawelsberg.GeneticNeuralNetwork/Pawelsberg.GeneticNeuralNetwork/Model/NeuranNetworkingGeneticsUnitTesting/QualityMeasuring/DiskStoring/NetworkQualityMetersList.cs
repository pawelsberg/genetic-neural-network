using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting.DiskStoring;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting.QualityMeasuring.DiskStoring;

public class NetworkQualityMetersList
{
    private const string Subdirectory = "QualityMeters";
    public const string Extension = "QualityMeters.txt";

    public static IEnumerable<string> GetNames()
    {
        string dirPath = Path.Combine(DataDirectory.FullPath, Subdirectory);
        if (!Directory.Exists(dirPath))
            return Enumerable.Empty<string>();

        string[] files = Directory.GetFiles(dirPath, "*." + Extension);

        for (int i = 0; i < files.Length; i++)
        {
            string file = files[i];
            file = file.Remove(file.LastIndexOf(Extension, StringComparison.InvariantCultureIgnoreCase) - 1);
            file = file.Split(Path.DirectorySeparatorChar).Last();
            files[i] = file;
        }
        return files;
    }

    public static QualityMeter<Network> LoadNetworkQualityMeters(string name, int propagations, TestCaseList testCaseList)
    {
        return NetworkQualityMetersExtension.Load(Path.Combine(DataDirectory.FullPath, Subdirectory, name + "." + Extension), propagations, testCaseList);
    }

    public static void Save(QualityMeter<Network> meter, string name)
    {
        meter.Save(Path.Combine(DataDirectory.FullPath, Subdirectory, name + "." + Extension));
    }

    public static void Delete(string name)
    {
        File.Delete(Path.Combine(DataDirectory.FullPath, Subdirectory, name + "." + Extension));
    }
}
