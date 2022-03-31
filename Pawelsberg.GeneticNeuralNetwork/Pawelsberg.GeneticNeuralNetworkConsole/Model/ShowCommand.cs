using System;
using System.Linq;
using Pawelsberg.GeneticNeuralNetwork.Model;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking.DiskStoring;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting.DiskStoring;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model
{

    public class ShowCommand : Command
    {
        public static string Name = "show";
        private bool _test;
        private bool _tests;
        private bool _vis;
        public override void LoadParameters(CodedText text)
        {
            if (!text.EOT)
            {
                string strText = text.ReadString();

                if (strText == "test")
                    _test = true;
                else if (strText == "tests")
                    _tests = true;
                else if (strText == "vis")
                    _vis = true;
                else
                    throw new Exception("Usage: show [test]");

                if (!text.EOT)
                    throw new Exception("Usage: show [test]");
            }

            base.LoadParameters(text);
        }

        public override void Run(NetworkSimulation simulation)
        {
            if (_test || _tests)
            {

                string format = _test ? "{0,6:F1}" : _tests ? "{0,1:F0}" : "{0} ";
                Console.ForegroundColor = ConsoleColor.Gray;

                Console.WriteLine("Best Ever Quality: {0}", simulation.BestEverQuality);

                TestCaseList testCaseList = simulation.TestCaseList;
                // inputs
                foreach (TestCase testCase in testCaseList.TestCases)
                {
                    Console.Write("Inputs:");
                    for (int i = 0; i < testCase.Inputs.Count; i++)
                    {
                        Console.Write(format, testCase.Inputs[i]);
                    }
                    Console.WriteLine();

                    Console.Write("ExpOut:");
                    for (int i = 0; i < testCase.Outputs.Count; i++)
                    {
                        Console.Write(format, testCase.Outputs[i]);
                    }
                    Console.WriteLine();

                    try
                    {
                        Network bestEver = simulation.BestEver;
                        RunningContext runningContext = bestEver.SafeRun(testCase, simulation.Propagations);

                        Console.Write("Output:");
                        for (int i = 0; i < testCase.Outputs.Count; i++)
                        {
                            double expout = testCase.Outputs[i];
                            double output = runningContext.GetPotential(bestEver.Outputs[i]);
                            if (Math.Abs(output - expout) > 0.001)
                                Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(format, output);
                            Console.ForegroundColor = ConsoleColor.Gray;

                        }
                        Console.WriteLine();
                    }
                    catch
                    {
                        Console.WriteLine("Running failed");
                    }

                    Console.WriteLine();

                }


            }
            else if (_vis)
            {
                // TODO
                //GeneticNeuralNetworkVisualiser.Model.NetworkExtension.ShowNetwork(simulation.BestEver);
            }
            else
            {
                Network bestEver = simulation.BestEver;
                if (bestEver != null)
                {
                    Console.WriteLine("Best Ever Network:");
                    Console.WriteLine(bestEver.ToString());
                }
                Console.WriteLine("Best Ever Quality: {0}", simulation.BestEverQuality);
                Console.WriteLine("Max Possible Quality: {0}", simulation.MaxPossibleQuality);
                Console.WriteLine("Generation: {0}", simulation.GenerationNumber);
                if (bestEver != null)
                {
                    Console.WriteLine("Best Ever Network (Nodes:{0},Synapses:{1})", bestEver.Nodes.Count, bestEver.GetAllSynapses().Count());
                    Console.WriteLine("Last Successful Mutations:");
                    foreach (string succesfulMutation in simulation.Log.LogItems)
                    {
                        Console.WriteLine(succesfulMutation);
                    }
                }

            }
        }
        public override string ShortDescription { get { return "Show the results of the genetic algorithm simulation"; } }
    }
}
