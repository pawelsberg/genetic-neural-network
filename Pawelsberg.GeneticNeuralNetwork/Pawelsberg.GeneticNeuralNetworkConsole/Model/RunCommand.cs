using Pawelsberg.GeneticNeuralNetwork.Model;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics.Simulating;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model;

public class RunCommand : Command
{
    public static string Name = "run";
    private int _generationCount;
    private string _fractionDoneFileName;
    private int? _fractionUpdateInterval;
    public override void LoadParameters(CodedText text)
    {
        _generationCount = text.ReadInt();
        if (_generationCount < 1)
        {
            throw new Exception("generation count should be grater then 0");
        }
        text.SkipWhiteCharacters();
        if (!text.EOT)
        {
            _fractionDoneFileName = text.ReadString();
        }
        text.SkipWhiteCharacters();
        if (!text.EOT)
        {
            _fractionUpdateInterval = text.ReadInt();
        }
        text.SkipWhiteCharacters();
        if (!text.EOT)
        {
            throw new Exception("Load command syntax exception - too many parameters");
        }

    }
    public override void Run(NetworkSimulation simulation)
    {
        int startingGeneration = simulation.GenerationNumber;
        int targetGeneration = startingGeneration + _generationCount;
        int currentGeneration;

        FileStream fileStream = null;
        TextWriter textWriter = null;
        try
        {
            if (_fractionDoneFileName != null)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_fractionDoneFileName));
                fileStream = new FileStream(_fractionDoneFileName, FileMode.Create, FileAccess.Write);
                textWriter = new StreamWriter(fileStream);
                Write(fileStream, textWriter, 0d);
            }

            EventHandler<NextGenerationCreatedEventArgs<Network>> generationAchievedEventHandler = (thisObject, nextGenerationEventArgs) =>
            {
                currentGeneration = simulation.GenerationNumber;

                if (_fractionDoneFileName != null && (!_fractionUpdateInterval.HasValue || (currentGeneration - startingGeneration) % _fractionUpdateInterval.Value == 0))
                    Write(fileStream, textWriter, (double)(currentGeneration - startingGeneration) / _generationCount);

                if (targetGeneration == currentGeneration)
                    nextGenerationEventArgs.Pause = true;
            };

            try
            {

                simulation.NextGenerationCreated += generationAchievedEventHandler;
                simulation.SimulationTimer.Start();
                while (simulation.SimulationTimer.Running)
                {
                    Thread.Sleep(1000);
                }

                if (_fractionDoneFileName != null)
                    Write(fileStream, textWriter, 1d);
            }
            finally
            {
                simulation.NextGenerationCreated -= generationAchievedEventHandler;
            }
        }
        finally
        {
            if (_fractionDoneFileName != null && textWriter != null)
                textWriter.Close();
        }

    }
    public override string ShortDescription { get { return "Start genetic algorithm for specific number of generations"; } }
    private static void Write(FileStream fileStream, TextWriter textWriter, double value)
    {
        fileStream.Seek(0, SeekOrigin.Begin);
        fileStream.SetLength(0);
        textWriter.Write(value);
        textWriter.Flush();
    }
}
