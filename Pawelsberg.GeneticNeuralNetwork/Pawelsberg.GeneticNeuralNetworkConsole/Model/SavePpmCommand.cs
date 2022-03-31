using System;
using Pawelsberg.GeneticNeuralNetwork.Model;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.DiskStoring;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting.DiskStoring;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model
{
    public class SavePpmCommand : Command
    {
        private string _name;
        public static string Name = "saveppm";
        public override void LoadParameters(CodedText text)
        {
            _name = text.ReadString();
            text.SkipWhiteCharacters();
            if (!text.EOT)
            {
                throw new Exception("Save ppm command syntax exception - too many parameters");
            }
        }
        public override void Run(NetworkSimulation simulation)
        {
            PpmCaseList ppmCaseList = simulation.TestCaseList as PpmCaseList;
            if (simulation.BestEver == null)
                throw new Exception("No best ever");
            if (ppmCaseList != null)
            {
                ppmCaseList.Save(simulation.BestEver, simulation.Propagations, _name);
            }
            else
            {
                throw new Exception("No ppm test case");
            }
        }
        public override string ShortDescription { get { return "Save the best neural network in ppm format"; } }
    }
}