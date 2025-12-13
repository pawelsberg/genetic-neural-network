namespace Pawelsberg.GeneticNeuralNetworkConsole.Model;

public class CommandDispatcher
{
    private Dictionary<string, Func<Command>> _commandNameCommands;
    public Dictionary<string, Func<Command>> CommandNameCommands { get { return _commandNameCommands; } }
    public CommandDispatcher()
    {
        _commandNameCommands = new Dictionary<string, Func<Command>>
        {
            {QuitCommand.Name, () => new QuitCommand()},
            {PauseCommand.Name, () => new PauseCommand()},
            {SetCommand.Name, () => new SetCommand()},
            {ShowCommand.Name, () => new ShowCommand()},
            {StartCommand.Name, () => new StartCommand()},
            {RunCommand.Name, () => new RunCommand()},
            {SaveCommand.Name, () => new SaveCommand()},
            {LoadCommand.Name, () => new LoadCommand()},
            {CreateCommand.Name, () => new CreateCommand()},
            {LoadAllCommand.Name, () => new LoadAllCommand()},
            {LoadClearCommand.Name, () => new LoadClearCommand()},
            {LoadTestCaseListCommand.Name, () => new LoadTestCaseListCommand()},
            {LoadSudokuCaseListCommand.Name, () => new LoadSudokuCaseListCommand()},
            {LoadPpmTestCaseListCommand.Name, () => new LoadPpmTestCaseListCommand()},
            {HelpCommand.Name, ()=> new HelpCommand(this)},
            {SavePpmCommand.Name, () => new SavePpmCommand()},
            {InitCommand.Name, () => new InitCommand()},
            {LoadMutatorsCommand.Name, () => new LoadMutatorsCommand()},
            {SaveMutatorsCommand.Name, () => new SaveMutatorsCommand()}
        };
    }
    public Command GetCommand(string commandName)
    {
        if (_commandNameCommands.ContainsKey(commandName))
            return _commandNameCommands[commandName]();
        throw new Exception(string.Format("Unknown command: \"{0}\"", commandName));
    }

}

