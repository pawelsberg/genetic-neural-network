using Pawelsberg.GeneticNeuralNetwork.Model;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model;

public abstract class Command
{
    public virtual void Run(NetworkSimulation simulation) { }
    public virtual void LoadParameters(CodedText text) { }
    public virtual string ShortDescription { get { return string.Empty; } }
    public virtual IEnumerable<string> GetParameterCompletions(string[] parameters) => Enumerable.Empty<string>();

    /// <summary>
    /// True when this command's Run() can change CPU simulation state in a way that the
    /// GPU simulation's snapshot (seeds, test cases, layout caps) was built from. MainMenu
    /// uses this to invalidate the GPU sim after the command runs so the next gpu* command
    /// rebuilds from current CPU state. Commands whose modifications don't translate to
    /// the GPU pipeline (mutators, quality meters — both hard-coded "Normal" on GPU) leave
    /// this false; SetCommand can flip it dynamically since `set` with no args just lists.
    /// </summary>
    public virtual bool InvalidatesGpuSimulation => false;
}


