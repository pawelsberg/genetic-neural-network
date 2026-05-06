using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model.Top;

public abstract class Mode
{
    public abstract string Name { get; }
    public abstract string KeyIndicator { get; }
    public abstract ConsoleKey Key { get; }
    public virtual string KeyHints => "";

    public abstract void Render(NetworkSimulation simulation, int availableLines, ScrollingPanel scrollingPanel);

    public virtual int GetTotalLines(NetworkSimulation simulation) => 0;

    public virtual bool HandleKey(ConsoleKey key, NetworkSimulation simulation, ScrollingPanel scrollingPanel, int availableLines) => false;
}
