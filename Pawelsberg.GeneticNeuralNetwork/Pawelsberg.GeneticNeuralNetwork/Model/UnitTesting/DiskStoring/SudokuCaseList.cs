namespace Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting.DiskStoring;

public class SudokuCaseList : TestCaseList
{
    public static readonly string Subdirectory = "SudokuCases";
    public const string Extension = "SudokuCase.txt";
    public SudokuCaseList()
    {
        foreach (string fileName in Directory.EnumerateFiles(Path.Combine(DataDirectory.FullPath, Subdirectory), "*." + Extension))
        {
            using (StreamReader streamReader = File.OpenText(fileName))
            {
                string firstLine = streamReader.ReadLine();
                string inputs = firstLine.Replace('.', '0');
                TestCase testCase = new TestCase();
                foreach (char inputChar in inputs)
                {
                    testCase.Inputs.Add(int.Parse(string.Format("{0}", inputChar)));
                }
                string outputs = streamReader.ReadLine();
                foreach (char outputChar in outputs)
                {
                    testCase.Outputs.Add(int.Parse(string.Format("{0}", outputChar)));
                }
                TestCases.Add(testCase);
            }
        }
    }
}
