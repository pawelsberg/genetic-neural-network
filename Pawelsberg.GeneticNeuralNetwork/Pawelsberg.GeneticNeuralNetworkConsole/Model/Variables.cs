using Pawelsberg.GeneticNeuralNetwork.Model;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model;

public static class Variables
{
    private static readonly Func<string, object> ParseInt = (text) => int.Parse(text);
    private static readonly Func<string, object> ParseParentQueuerType = (text) => Enum.Parse(typeof(ParentQueuerType), text);

    public static IReadOnlyList<Variable> List { get; } = new List<Variable>
    {
        new Variable { Name = "maxNodes", Getter = (sim) => sim.MaxNodes, Setter = (sim, val) => sim.MaxNodes = (int)val, Parse = ParseInt },
        new Variable { Name = "maxSynapses", Getter = (sim) => sim.MaxSynapses, Setter = (sim, val) => sim.MaxSynapses = (int)val, Parse = ParseInt },
        new Variable { Name = "propagations", Getter = (sim) => sim.Propagations, Setter = (sim, val) => sim.Propagations = (int)val, Parse = ParseInt },
        new Variable { Name = "successfulMutationsLength", Getter = (sim) => sim.Log.MaxLength, Setter = (sim, val) => sim.Log.MaxLength = (int)val, Parse = ParseInt },
        new Variable { Name = "maxSpecimens", Getter = (sim) => sim.MaxSpecimens, Setter = (sim, val) => sim.MaxSpecimens = (int)val, Parse = ParseInt },
        new Variable { Name = "delayTimeMs", Getter = (sim) => sim.SimulationTimer.DelayTimeMs, Setter = (sim, val) => sim.SimulationTimer.DelayTimeMs = (int)val, Parse = ParseInt },
        new Variable { Name = "generationMultiplier", Getter = (sim) => sim.GenerationMultiplier, Setter = (sim, val) => sim.GenerationMultiplier = (int)val, Parse = ParseInt },
        new Variable { Name = "parentQueuer", Getter = (sim) => sim.ParentQueuerType, Setter = (sim, val) => sim.ParentQueuerType = (ParentQueuerType)val, Parse = ParseParentQueuerType },
        new Variable { Name = "seed", Getter = (sim) => "x", Setter = (sim, val) => RandomGenerator.Random = new Random((int)val), Parse = ParseInt }
    };

    public static IEnumerable<string> Names => List.Select(v => v.Name);
}
