using Pawelsberg.GeneticNeuralNetwork.Model;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model;

public class MainMenu
{
    private NetworkSimulation _simulation;
    private ConsoleInputReader _inputReader;

    public MainMenu(NetworkSimulation simulation)
    {
        _simulation = simulation;
        _inputReader = new ConsoleInputReader();
    }
    public void Run()
    {
        Command command = null;
        while (!(command is QuitCommand))
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(">");
            string inputText = _inputReader.ReadLine();
            if (inputText == null)
                break;
            Console.ResetColor();
            CodedText codedText = new CodedText(inputText);

            codedText.SkipWhiteCharacters();
            string commandName = codedText.ReadString();

            try
            {
                command = Commands.GetCommand(commandName);
                codedText.SkipWhiteCharacters();
                command.LoadParameters(codedText);
                command.Run(_simulation);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

