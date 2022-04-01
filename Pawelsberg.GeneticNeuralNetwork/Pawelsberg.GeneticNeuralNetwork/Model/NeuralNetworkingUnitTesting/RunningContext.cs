using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting;

public class RunningContext
{
    public Dictionary<Synapse, double> SynapsePotentials { get; set; }

    public RunningContext()
    {
        SynapsePotentials = new Dictionary<Synapse, double>();
    }

    public void SetPotential(Synapse synapse, double potential)
    {
        SynapsePotentials[synapse] = potential;
    }
    public double GetPotential(Synapse synapse)
    {
        if (!SynapsePotentials.ContainsKey(synapse))
            throw new Exception("Synapse not found");
        return SynapsePotentials[synapse];
    }
}
