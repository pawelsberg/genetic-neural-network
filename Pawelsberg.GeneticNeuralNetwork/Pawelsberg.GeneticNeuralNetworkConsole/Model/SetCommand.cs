using Pawelsberg.GeneticNeuralNetwork.Model;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model;

public class SetCommand : Command
{
    public static string Name = "set";
    private bool _listSets;
    private string _variableName;
    private bool _displayVariable;
    private string _value;
    private List<Variable> _variableList;

    public SetCommand()
    {
        Func<string, object> parseInt = (text) => int.Parse(text);
        Func<string, object> parseMeterType = (text) => Enum.Parse(typeof(MeterType), text);
        Func<string, object> parseParentQueuerType = (text) => Enum.Parse(typeof(ParentQueuerType), text);

        _variableList = new List<Variable>()
        {
            new Variable() { Name = "maxNodes", Getter = (sim)=> sim.MaxNodes, Setter = (sim,val)=> sim.MaxNodes = (int)val, Parse = parseInt},
            new Variable() { Name = "maxSynapses", Getter = (sim)=> sim.MaxSynapses, Setter = (sim,val)=> sim.MaxSynapses = (int)val, Parse = parseInt },
            new Variable() { Name = "propagations", Getter = (sim)=> sim.Propagations, Setter = (sim,val)=> sim.Propagations = (int)val, Parse = parseInt },
            new Variable() { Name = "successfulMutationsLength", Getter = (sim)=> sim.Log.MaxLength, Setter = (sim,val)=> sim.Log.MaxLength = (int)val, Parse = parseInt },
            new Variable() { Name = "maxSpecimens", Getter = (sim)=> sim.MaxSpecimens, Setter = (sim,val)=> sim.MaxSpecimens = (int)val, Parse = parseInt },
            new Variable() { Name = "delayTimeMs", Getter = (sim)=> sim.SimulationTimer.DelayTimeMs, Setter = (sim,val)=> sim.SimulationTimer.DelayTimeMs = (int)val, Parse = parseInt },
            new Variable() { Name = "generationMultiplier", Getter = (sim)=> sim.GenerationMultiplier, Setter = (sim,val)=> sim.GenerationMultiplier = (int)val, Parse = parseInt },
            new Variable() { Name = "meterType", Getter = (sim)=> sim.MeterType, Setter = (sim,val)=> sim.MeterType = (MeterType)val, Parse = parseMeterType },
            new Variable() { Name = "parentQueuer", Getter = (sim)=> sim.ParentQueuerType, Setter = (sim,val)=> sim.ParentQueuerType = (ParentQueuerType)val, Parse = parseParentQueuerType },
            new Variable() { Name = "seed", Getter = (sim)=> "x", Setter = (sim,val)=> RandomGenerator.Random = new Random((int)val), Parse = parseInt }
        };
    }

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
            foreach (Variable variable in _variableList)
                variable.Display(simulation);
        }
        else if (_displayVariable)
        {
            Variable variable = _variableList.FirstOrDefault(varble => varble.Name == _variableName);
            if (variable != null)
                variable.Display(simulation);
            else
                throw new Exception(string.Format("Variable {0} not found", _variableName));
        }
        else
        {
            Variable variable = _variableList.FirstOrDefault(varble => varble.Name == _variableName);
            if (variable != null)
                variable.Setter(simulation, variable.Parse(_value));
            else
                throw new Exception(string.Format("Variable {0} not found", _variableName));
        }
    }
    public override string ShortDescription { get { return "Show/set general settings of genetic algorithm"; } }
}

