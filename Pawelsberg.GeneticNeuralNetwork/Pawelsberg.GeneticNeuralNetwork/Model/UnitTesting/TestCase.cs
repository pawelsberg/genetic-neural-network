using System.Collections.Generic;

namespace Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting
{
    public class TestCase
    {
        public List<int> Inputs { get; set; }
        public List<int> Outputs { get; set; }

        public TestCase()
        {
            Inputs = new List<int>();
            Outputs = new List<int>();
        }
    }
}
