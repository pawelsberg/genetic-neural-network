using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting.QualityMeasuring.DiskStoring;

public static class NetworkQualityMetersExtension
{
    public static void Save(this QualityMeter<Network> meter, string fileName)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(fileName));
        using (FileStream fileStream = File.Open(fileName, FileMode.Create))
        using (StreamWriter streamWriter = new StreamWriter(fileStream))
        {
            streamWriter.Write(meter.ToText());
            streamWriter.Flush();
        }
    }

    public static QualityMeter<Network> Load(string fileName, int propagations, TestCaseList testCaseList)
    {
        using (FileStream fileStream = File.Open(fileName, FileMode.Open))
        using (StreamReader streamReader = new StreamReader(fileStream))
        {
            string text = streamReader.ReadToEnd();
            return NetworkQualityMetersTextExtension.Parse(text, propagations, testCaseList);
        }
    }
}
