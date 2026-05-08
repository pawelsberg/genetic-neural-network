using Pawelsberg.GeneticNeuralNetwork.Model.Genetics.Simulating;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
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

        // Reproduces the user's regression sequence:
        //   loadtcl <tcl> ; loadall ; set propagations <p> ; run <n> ; gpurun
        if (args.Length > 0 && args[0] == "--regression")
        {
            string tclName = args.Length > 1 ? args[1] : "mul3";
            int propagations = args.Length > 2 ? int.Parse(args[2]) : 20;
            int genCount = args.Length > 3 ? int.Parse(args[3]) : 1;

            new InitCommand().Run(simulation);
            simulation.TestCaseList = TestCaseLists.LoadTestCaseList(tclName);
            new LoadAllCommand().Run(simulation);
            simulation.Propagations = propagations;
            simulation.GenerationMultiplier = 1;

            Console.WriteLine($"--regression: tcl={tclName} propagations={propagations} cpuGens={genCount}");

            int startGen = simulation.GenerationNumber;
            int targetGen = startGen + genCount;
            EventHandler<NextGenerationCreatedEventArgs<Network>> handler = (sender, evtArgs) =>
            {
                if (simulation.GenerationNumber >= targetGen)
                    evtArgs.Pause = true;
            };
            simulation.NextGenerationCreated += handler;
            simulation.SimulationTimer.Start();
            while (simulation.SimulationTimer.Running)
                Thread.Sleep(20);
            simulation.NextGenerationCreated -= handler;

            Console.WriteLine($"After CPU run: GenerationNumber={simulation.GenerationNumber}, BestEverQuality={simulation.BestEverQuality:F6}, MaxPossibleQuality={simulation.MaxPossibleQuality:F6}");
            new GpuRunCommand().Run(simulation);
            return;
        }

        MainMenu mainMenu = new MainMenu(simulation);
        mainMenu.Run();
    }
}
