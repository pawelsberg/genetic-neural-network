namespace Pawelsberg.GeneticNeuralNetwork.Model;

public static class DataDirectory
{
    public static string FullPath = Path.Combine( Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), $"{nameof(Pawelsberg)}.{nameof(GeneticNeuralNetwork)}");
}
