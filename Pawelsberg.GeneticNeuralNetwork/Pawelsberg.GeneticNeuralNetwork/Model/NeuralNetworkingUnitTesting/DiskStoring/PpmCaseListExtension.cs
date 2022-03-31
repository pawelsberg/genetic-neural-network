using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting.DiskStoring;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.DiskStoring
{
    public static class PpmCaseListExtension
    {
        public static void Save(this PpmCaseList thisPpmCaseList, Network network, int propagations, string name)
        {
            using (FileStream fileStream = File.Open(Directory.GetCurrentDirectory() + PpmCaseList.Subdirectory + Path.DirectorySeparatorChar + name + "." + PpmCaseList.Extension, FileMode.Create))
            using (StreamWriter streamWriter = new StreamWriter(fileStream))
            {
                streamWriter.WriteLine("P3");
                int width = thisPpmCaseList.TestCases.Max(tc => tc.Outputs.Count) / 3;
                int height = thisPpmCaseList.TestCases.Count;
                streamWriter.WriteLine("{0} {1}", width, height);
                streamWriter.WriteLine(255);
                for (int y = 0; y < height; y++)
                {
                    RunningContext runningContext = network.SafeRun(thisPpmCaseList.TestCases[y], propagations);
                    for (int x = 0; x < width; x++)
                    {
                        if (network.Outputs.Count > x * 3 + 2)
                        {
                            int c;
                            c = (int)Math.Min(Math.Max(runningContext.SynapsePotentials[network.Outputs[x * 3]], 0), 255);
                            streamWriter.WriteLine(c);
                            c = (int)Math.Min(Math.Max(runningContext.SynapsePotentials[network.Outputs[x * 3 + 1]], 0), 255);
                            streamWriter.WriteLine(c);
                            c = (int)Math.Min(Math.Max(runningContext.SynapsePotentials[network.Outputs[x * 3 + 2]], 0), 255);
                            streamWriter.WriteLine(c);
                        }
                        else
                        {
                            streamWriter.WriteLine(0);
                            streamWriter.WriteLine(0);
                            streamWriter.WriteLine(0);
                        }
                    }
                }

                streamWriter.Flush();
            }



        }

    }
}
