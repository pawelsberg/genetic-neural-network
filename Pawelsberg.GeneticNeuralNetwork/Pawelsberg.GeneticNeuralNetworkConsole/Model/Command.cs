using System;
using Pawelsberg.GeneticNeuralNetwork.Model;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model
{
    public abstract class Command
    {
        public virtual void Run(NetworkSimulation simulation) { }
        public virtual void LoadParameters(CodedText text) { }
        public virtual string ShortDescription { get { return string.Empty; } }
    }

}

