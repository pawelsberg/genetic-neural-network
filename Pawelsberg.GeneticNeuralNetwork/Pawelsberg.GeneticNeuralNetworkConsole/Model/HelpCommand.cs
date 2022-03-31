using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pawelsberg.GeneticNeuralNetwork.Model;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model
{
    public class HelpCommand : Command
    {
        public static string Name = "help";
        private readonly CommandDispatcher _commandDispatcher;
        private string _commandName;
        private string _commandShortDescription;
        public HelpCommand(CommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

        public override void LoadParameters(CodedText text)
        {
            if (!text.EOT)
            {
                string strText = text.ReadString();

                if (_commandDispatcher.CommandNameCommands.ContainsKey(strText))
                {
                    _commandName = strText;
                    Command command = _commandDispatcher.CommandNameCommands[strText]();
                    _commandShortDescription = command.ShortDescription;
                }
                else
                    throw new Exception("Usage: help [command]");
            }

            base.LoadParameters(text);
        }

        public override void Run(NetworkSimulation simulation)
        {
            if (_commandName == null)
            {
                StringBuilder helpStringBuilder = new StringBuilder();
                foreach (KeyValuePair<string, Func<Command>> comandNameCommand in _commandDispatcher.CommandNameCommands.OrderBy(cnc => cnc.Key))
                {
                    string commandName = comandNameCommand.Key;
                    string commandShortDescription = comandNameCommand.Value().ShortDescription;
                    helpStringBuilder.AppendLine($"{commandName,-13} {commandShortDescription}");
                }

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(helpStringBuilder.ToString());
            }
            else
            {
                Console.WriteLine($"{_commandName} - {_commandShortDescription}");
            }
        }
        public override string ShortDescription { get { return "Show help"; } }
    }
}