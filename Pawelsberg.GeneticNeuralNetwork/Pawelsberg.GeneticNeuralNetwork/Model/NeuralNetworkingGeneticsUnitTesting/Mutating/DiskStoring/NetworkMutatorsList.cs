using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.Mutating.DiskStoring
{
    public class NetworkMutatorsList
    {
        private const string Subdirectory = "Mutators";
        public const string Extension = "Mutators.txt";

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

        public static NetworkMutators Load(string name, int maxNodes, int maxSynapses, int propagations, TestCaseList testCaseList)
        {
            return NetworkMutatorsExtension.Load(Path.Combine(DataDirectory.FullPath, Subdirectory, name + "." + Extension), maxNodes, maxSynapses, propagations, testCaseList);
        }

        public static void Save(NetworkMutators mutators, string name)
        {
            mutators.Save(Path.Combine(DataDirectory.FullPath, Subdirectory, name + "." + Extension));
        }

        public static void Delete(string name)
        {
            File.Delete(Path.Combine(DataDirectory.FullPath, Subdirectory, name + "." + Extension));
        }
    }
}
