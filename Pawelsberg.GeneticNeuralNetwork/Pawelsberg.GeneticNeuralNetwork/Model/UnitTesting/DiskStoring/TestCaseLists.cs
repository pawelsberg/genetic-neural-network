namespace Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting.DiskStoring;

public static class TestCaseLists
{
    public static readonly string Subdirectory = "TestCases";
    public const string Extension = "TestCases.XML";
    public static IEnumerable<string> GetNames()
    {
        string[] files = Directory.GetFiles(Path.Combine(DataDirectory.FullPath, Subdirectory), "*." + Extension);

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
        return TestCaseListExtension.Load(Path.Combine(DataDirectory.FullPath, Subdirectory, name + "." + Extension));
    }

    public static void Save(TestCaseList testCase, string name)
    {
        testCase.Save(Path.Combine(DataDirectory.FullPath, Subdirectory, name + "." + Extension));
    }

    public static void Delete(string name)
    {
        File.Delete(Path.Combine(DataDirectory.FullPath, Subdirectory, name + "." + Extension));
    }
}
