namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking.DiskStoring;

public static class NetworkList
{
    public static readonly string Subdirectory = "Networks";
    public const string Extension = "Network.txt";
    public static IEnumerable<string> GetNames()
    {
        string[] files = Directory.GetFiles(Path.Combine(DataDirectory.FullPath, Subdirectory), "*." + Extension);

        for (int i = 0; i < files.Length; i++)
        {
            string file = files[i];
            file = file.Remove(file.LastIndexOf(Extension, StringComparison.InvariantCultureIgnoreCase) - 1);
            file = file.Split(Path.DirectorySeparatorChar).Last();
            files[i] = file;
        }
        return files;
    }

    public static Network LoadNetwork(string name)
    {
        return NetworkExtension.Load(Path.Combine(DataDirectory.FullPath, Subdirectory, name + "." + Extension));
    }

    public static void Save(Network network, string name)
    {
        network.Save(Path.Combine(DataDirectory.FullPath, Subdirectory, name + "." + Extension));
    }

    public static void Delete(string name)
    {
        File.Delete(Path.Combine(DataDirectory.FullPath, Subdirectory, name + "." + Extension));
    }

    public static IEnumerable<Network> LoadAll()
    {
        List<Network> networks = new List<Network>();
        foreach (string filePath in Directory.GetFiles(Path.Combine(DataDirectory.FullPath, Subdirectory), "*." + Extension))
        {
            networks.Add(NetworkExtension.Load(filePath));
        }
        return networks;
    }
}
