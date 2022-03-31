using System;
using Pawelsberg.GeneticNeuralNetwork.Model;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking.DiskStoring;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting.DiskStoring;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model
{
    public class LoadSudokuCaseListCommand : Command
    {
        public static string Name = "loadsudokutcl";
        public override void LoadParameters(CodedText text)
        {
            if (!text.EOT)
            {
                throw new Exception("Sudoku do not need parameters");
            }
        }
        public override void Run(NetworkSimulation simulation)
        {
            TestCaseList testCaseList = new SudokuCaseList();
            simulation.TestCaseList = testCaseList;
        }
        public override string ShortDescription { get { return "Loads a test case list in sudoku format"; } }
    }

}