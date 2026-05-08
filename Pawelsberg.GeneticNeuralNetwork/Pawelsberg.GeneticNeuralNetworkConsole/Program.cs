using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting.DiskStoring;
using Pawelsberg.GeneticNeuralNetworkConsole.Model;

namespace Pawelsberg.GeneticNeuralNetworkConsole;

class Program
{
    static void Main(string[] args)
    {
        NetworkSimulation simulation = new NetworkSimulation()
        //{
        //    TestCaseList = TestCaseLists.LoadTestCaseList("default")
        //}
        ;

        if (args.Length > 0 && args[0] == "--gpurun")
        {
            new InitCommand().Run(simulation);
            new LoadAllCommand().Run(simulation);
            if (args.Length > 1)
            {
                string tclName = args[1];
                Console.WriteLine($"Loading test case list: {tclName}");
                simulation.TestCaseList = TestCaseLists.LoadTestCaseList(tclName);
            }
            new GpuRunCommand().Run(simulation);
            return;
        }

        MainMenu mainMenu = new MainMenu(simulation);
        mainMenu.Run();
    }
}
