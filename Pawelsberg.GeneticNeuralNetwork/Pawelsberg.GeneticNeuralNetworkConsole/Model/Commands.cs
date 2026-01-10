namespace Pawelsberg.GeneticNeuralNetworkConsole.Model;

public static class Commands
{
    public static IReadOnlyDictionary<string, Func<Command>> NameCommands { get; } = new Dictionary<string, Func<Command>>
    {
        { QuitCommand.Name, () => new QuitCommand() },
        { PauseCommand.Name, () => new PauseCommand() },
        { SetCommand.Name, () => new SetCommand() },
        { ShowCommand.Name, () => new ShowCommand() },
        { StartCommand.Name, () => new StartCommand() },
        { RunCommand.Name, () => new RunCommand() },
        { SaveCommand.Name, () => new SaveCommand() },
        { LoadCommand.Name, () => new LoadCommand() },
        { CreateCommand.Name, () => new CreateCommand() },
        { LoadAllCommand.Name, () => new LoadAllCommand() },
        { LoadClearCommand.Name, () => new LoadClearCommand() },
        { LoadTestCaseListCommand.Name, () => new LoadTestCaseListCommand() },
        { LoadSudokuCaseListCommand.Name, () => new LoadSudokuCaseListCommand() },
        { LoadPpmTestCaseListCommand.Name, () => new LoadPpmTestCaseListCommand() },
        { HelpCommand.Name, () => new HelpCommand() },
        { SavePpmCommand.Name, () => new SavePpmCommand() },
        { InitCommand.Name, () => new InitCommand() },
        { LoadMutatorsCommand.Name, () => new LoadMutatorsCommand() },
        { SaveMutatorsCommand.Name, () => new SaveMutatorsCommand() },
        { LoadQualityMetersCommand.Name, () => new LoadQualityMetersCommand() },
        { SaveQualityMetersCommand.Name, () => new SaveQualityMetersCommand() }
    };

    public static IEnumerable<string> Names => NameCommands.Keys;

    public static Command GetCommand(string commandName)
    {
        if (NameCommands.ContainsKey(commandName))
            return NameCommands[commandName]();
        throw new Exception(string.Format("Unknown command: \"{0}\"", commandName));
    }
}
