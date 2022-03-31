using System;
using Pawelsberg.GeneticNeuralNetwork.Model;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model
{
    public class MainMenu
    {
        private NetworkSimulation _simulation;
        private CommandDispatcher _commandDispatcher;
        public MainMenu(NetworkSimulation simulation)
        {
            _simulation = simulation;
            _commandDispatcher = new CommandDispatcher();
        }
        public void Run()
        {
            Command command = null;
            while (!(command is QuitCommand))
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(">");
                string inputText = Console.ReadLine();
                if (inputText == null)
                    break;
                Console.ResetColor();
                CodedText codedText = new CodedText(inputText);

                codedText.SkipWhiteCharacters();
                string commandName = codedText.ReadString();

                try
                {
                    command = _commandDispatcher.GetCommand(commandName);
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
}

