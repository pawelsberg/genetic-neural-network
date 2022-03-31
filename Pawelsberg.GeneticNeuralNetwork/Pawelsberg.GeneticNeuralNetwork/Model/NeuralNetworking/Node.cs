namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking
{
    public abstract class Node
    {
        public List<Synapse> Outputs { get; private set; }

        protected Node()
        {
            Outputs = new List<Synapse>();
        }
        public void AddOutput(Synapse synapse)
        {
            Outputs.Add(synapse);
        }
        public void RemoveOutput(Synapse synapse)
        {
            int synapseIndex = Outputs.IndexOf(synapse);
            Outputs.RemoveAt(synapseIndex);
        }
    }
}
