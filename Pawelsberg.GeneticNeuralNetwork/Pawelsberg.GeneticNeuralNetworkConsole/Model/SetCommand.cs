using Pawelsberg.GeneticNeuralNetwork.Model;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model;

public class SetCommand : Command
{
    public static string Name = "set";
    private bool _listSets;
    private string _variableName;
    private bool _displayVariable;
    private string _value;

    public override void LoadParameters(CodedText text)
    {
        if (text.EOT)
        {
            _listSets = text.EOT;
            return;
        }
        _variableName = text.ReadString();
        text.SkipWhiteCharacters();
        if (text.EOT)
        {
            _displayVariable = true;
            return;
        }
        _value = text.ReadString();
        text.SkipWhiteCharacters();
        if (!text.EOT)
        {
            throw new Exception("Set command syntax exception - too many parameters");
        }
    }
    public override void Run(NetworkSimulation simulation)
    {
        if (_listSets)
        {
            foreach (Variable variable in Variables.List)
                variable.Display(simulation);
        }
        else if (_displayVariable)
        {
            Variable variable = Variables.List.FirstOrDefault(varble => varble.Name == _variableName);
            if (variable != null)
                variable.Display(simulation);
            else
                throw new Exception(string.Format("Variable {0} not found", _variableName));
        }
        else
        {
            Variable variable = Variables.List.FirstOrDefault(varble => varble.Name == _variableName);
            if (variable != null)
                variable.Setter(simulation, variable.Parse(_value));
            else
                throw new Exception(string.Format("Variable {0} not found", _variableName));
        }
    }
    public override string ShortDescription { get { return "Show/set general settings of genetic algorithm"; } }
    public override IEnumerable<string> GetParameterCompletions(string[] parameters)
    {
        if (parameters.Length <= 1)
            return Variables.Names;
        if (parameters.Length == 2 && parameters[0].Equals("parentQueuer", StringComparison.OrdinalIgnoreCase))
            return Enum.GetNames(typeof(ParentQueuerType));
        return Enumerable.Empty<string>();
    }
}

