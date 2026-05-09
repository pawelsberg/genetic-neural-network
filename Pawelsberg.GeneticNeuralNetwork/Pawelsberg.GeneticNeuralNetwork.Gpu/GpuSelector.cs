using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Pawelsberg.GeneticNeuralNetwork.Gpu
{
    public static class GpuSelector
    {
        [DllImport("nvapi64.dll", EntryPoint = "fake")]
        private static extern int LoadNvApi64();

        [DllImport("nvapi.dll", EntryPoint = "fake")]
        private static extern int LoadNvApi32();

        public static void ForceDedicatedGpu()
        {
            try
            {
                if (Environment.Is64BitProcess)
                    LoadNvApi64();
                else
                    LoadNvApi32();

                Console.WriteLine("Requested NVIDIA discrete GPU via nvapi preload.");
            }
            catch
            {
                // Expected - we use fake entry point just to load the driver
            }
        }
    }
}
