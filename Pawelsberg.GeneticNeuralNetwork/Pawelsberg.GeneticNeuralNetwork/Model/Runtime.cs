using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pawelsberg.GeneticNeuralNetwork.Model
{
    public static class Runtime
    {
        public static bool IsRunningOnMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }
    }
}
