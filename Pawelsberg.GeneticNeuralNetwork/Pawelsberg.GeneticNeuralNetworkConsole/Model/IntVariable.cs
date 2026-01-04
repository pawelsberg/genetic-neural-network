using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model;

public class Variable
{
    public string Name { get; set; }
    public Func<NetworkSimulation, object> Getter { get; set; }
    public Action<NetworkSimulation, object> Setter { get; set; }
    public Func<string, object> Parse { get; set; }
    public void Display(NetworkSimulation simulation)
    {
        Console.WriteLine("{0}: {1}", Name, Getter(simulation));
    }
}

