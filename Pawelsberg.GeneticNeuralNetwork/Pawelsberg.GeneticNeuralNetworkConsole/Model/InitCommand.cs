using Pawelsberg.GeneticNeuralNetwork.Model;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting;
using System.IO;
using System.Reflection;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model;

public class InitCommand : Command
{
    public static string Name = "init";

    public override void Run(NetworkSimulation simulation)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        string resourcePrefix = $"{assembly.GetName().Name}.Resources.InitialData.";

        foreach (string resourceName in assembly.GetManifestResourceNames().Where(rn => rn.StartsWith(resourcePrefix)))
        {
            string relativePath = resourceName.Substring(resourcePrefix.Length);
            string[] parts = relativePath.Split('.');

            // First part is subfolder, rest is filename
            if (parts.Length < 2)
                continue;

            string folder = parts[0];
            string fileName = string.Join(".", parts.Skip(1));

            string targetDir = Path.Combine(DataDirectory.FullPath, folder);
            
            if (!Directory.Exists(targetDir))
                Directory.CreateDirectory(targetDir);

            string targetPath = Path.Combine(targetDir, fileName);

            if (!File.Exists(targetPath))
            {
                using Stream? stream = assembly.GetManifestResourceStream(resourceName);
                if (stream != null)
                {
                    using FileStream fileStream = File.Create(targetPath);
                    stream.CopyTo(fileStream);
                }
            }
        }

        Console.WriteLine("Initialising resources");
    }

    public override string ShortDescription { get { return "Extract embedded resources to AppData"; } }
}
