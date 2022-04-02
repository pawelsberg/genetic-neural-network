namespace Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting.DiskStoring;

public class PpmCaseList : TestCaseList
{
    public static readonly string Subdirectory = "PpmCases";
    public const string Extension = "ppm";
    public PpmCaseList(string name)
    {
        using (StreamReader streamReader = File.OpenText(Path.Combine(DataDirectory.FullPath, Subdirectory, name + "." + Extension)))
        {
            streamReader.ReadLine(); // P3
            streamReader.ReadLine(); // Comment
            CodedText codedText = new CodedText(streamReader.ReadToEnd());
            int width = codedText.ReadInt();
            codedText.SkipWhiteCharacters();
            int height = codedText.ReadInt();
            codedText.SkipWhiteCharacters();

            for (int y = 0; y < height; y++)
            {
                TestCase testCase = new TestCase();
                testCase.Inputs.Add(y + 1);
                for (int x = 0; x < width * 3; x++)
                {
                    int c = codedText.ReadInt();
                    codedText.SkipWhiteCharacters();
                    testCase.Outputs.Add(c);
                }
                TestCases.Add(testCase);
            }
        }
    }
}
