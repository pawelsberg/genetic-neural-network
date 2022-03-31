using System;
using Pawelsberg.GeneticNeuralNetwork.Model;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting.DiskStoring;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model
{
    public class LoadPpmTestCaseListCommand : Command
    {
        private string _name;
        public static string Name = "loadppmtcl";
        public override void LoadParameters(CodedText text)
        {
            _name = text.ReadString();
            text.SkipWhiteCharacters();
            if (!text.EOT)
            {
                throw new Exception("Load PPM Test Case List command syntax exception - too many parameters");
            }
        }
        public override void Run(NetworkSimulation simulation)
        {
            TestCaseList testCaseList = new PpmCaseList(_name);
            simulation.TestCaseList = testCaseList;
        }
        public override string ShortDescription { get { return "Loads a test case list in ppm format"; } }
    }
}