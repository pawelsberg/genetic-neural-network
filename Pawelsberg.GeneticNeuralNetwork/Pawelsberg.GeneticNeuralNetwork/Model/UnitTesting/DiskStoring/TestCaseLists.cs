using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting.DiskStoring
{
    public static class TestCaseLists
    {
        public static readonly string Subdirectory = Path.DirectorySeparatorChar + "TestCases";
        public const string Extension = "TestCases.XML";
        public static IEnumerable<string> GetNames()
        {
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + Subdirectory, "*." + Extension);

            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                file = file.Remove(file.LastIndexOf(Extension, StringComparison.InvariantCultureIgnoreCase) - 1);
                file = file.Split(Path.DirectorySeparatorChar).Last();
                files[i] = file;
            }

            return files;
        }

        public static TestCaseList LoadTestCaseList(string name)
        {
            return TestCaseList.Load(Directory.GetCurrentDirectory() + Subdirectory + Path.DirectorySeparatorChar + name + "." + Extension);
        }

        public static void Save(TestCaseList testCase, string name)
        {
            testCase.Save(Directory.GetCurrentDirectory() + Subdirectory + Path.DirectorySeparatorChar + name + "." + Extension);
        }

        public static void Delete(string name)
        {
            File.Delete(Directory.GetCurrentDirectory() + Subdirectory + Path.DirectorySeparatorChar + name + "." + Extension);
        }
    }
}
