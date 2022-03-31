using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting.DiskStoring;

namespace Pawelsberg.GeneticNeuralNetwork.Model
{
    public static class TestCasesList
    {
        public const string Subdirectory = @"\TestCases";
        public const string Extension = @"TestCases.XML";
        public static IEnumerable<string> GetNames()
        {
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + Subdirectory, "*." + Extension);

            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                file = file.Remove(file.LastIndexOf(Extension, StringComparison.InvariantCultureIgnoreCase) - 1);
                file = file.Split('\\').Last();
                files[i] = file;
            }

            return files;
        }

        public static TestCaseList LoadTestCaseList(string name)
        {
            return TestCaseList.Load(Directory.GetCurrentDirectory() + Subdirectory + @"\" + name + "." + Extension);
        }

        public static void Save(TestCaseList testCase, string name)
        {
            testCase.Save(Directory.GetCurrentDirectory() + Subdirectory + @"\" + name + "." + Extension);
        }

        public static void Delete(string name)
        {
            File.Delete(Directory.GetCurrentDirectory() + Subdirectory + @"\" + name + "." + Extension);
        }
    }
}
