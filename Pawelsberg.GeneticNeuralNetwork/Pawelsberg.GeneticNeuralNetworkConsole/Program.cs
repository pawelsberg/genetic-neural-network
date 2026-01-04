using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting;
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

        MainMenu mainMenu = new MainMenu(simulation);
        mainMenu.Run();
    }
}
