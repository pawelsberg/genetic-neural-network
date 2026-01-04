using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.Mutating.DiskStoring;

public static class NetworkMutatorsExtension
{
    public static void Save(this NetworkMutators mutators, string fileName)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(fileName));
        using (FileStream fileStream = File.Open(fileName, FileMode.Create))
        using (StreamWriter streamWriter = new StreamWriter(fileStream))
        {
            streamWriter.Write(mutators.ToText());
            streamWriter.Flush();
        }
    }
    public static NetworkMutators Load(string fileName, int maxNodes, int maxSynapses, int propagations, TestCaseList testCaseList)
    {
        using (FileStream fileStream = File.Open(fileName, FileMode.Open))
        using (StreamReader streamReader = new StreamReader(fileStream))
        {
            string text = streamReader.ReadToEnd();
            return NetworkMutatorsTextExtension.Parse(text, maxNodes, maxSynapses, propagations, testCaseList);
        }
    }
}
