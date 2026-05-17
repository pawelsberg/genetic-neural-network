using Pawelsberg.GeneticNeuralNetwork.Model;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model;

public class MainMenu
{
    private NetworkSimulation _simulation;
    private CommandInput _commandInput;
    private readonly bool _inputRedirected = Console.IsInputRedirected;

    public MainMenu(NetworkSimulation simulation)
    {
        _simulation = simulation;
        _commandInput = new CommandInput();
    }
    public void Run()
    {
        Command command = null;
        while (!(command is QuitCommand))
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(">");
            // When stdin is piped (e.g. the web-export generator), Console.ReadKey throws,
            // so fall back to plain line reading without completion/history.
            string inputText = _inputRedirected ? Console.In.ReadLine() : _commandInput.ReadLine();
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

