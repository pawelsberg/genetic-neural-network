using System;
using System.Collections.Generic;
using Pawelsberg.GeneticNeuralNetwork.Model;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking.DiskStoring;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model
{
    public class LoadAllCommand : Command
    {
        public static string Name = "loadall";
        public override void LoadParameters(CodedText text)
        {
            text.SkipWhiteCharacters();
            if (!text.EOT)
            {
                throw new Exception("LoadAll command syntax exception - too many parameters");
            }
        }
        public override void Run(NetworkSimulation simulation)
        {
            IEnumerable<Network> networkEnumerable = NetworkList.LoadAll();

            foreach (Network network in networkEnumerable)
            {
                network.ToString();
                simulation.Add(network);
            }
        }
        public override string ShortDescription { get { return "Loads all neural networks as additional specimens"; } }
    }
}