using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking
{
    public class Network : ISpecimen
    {
        public List<Node> Nodes { get; private set; }
        public List<Synapse> Inputs { get; private set; }
        public List<Synapse> Outputs { get; private set; }
        public MutationDescription MutationDescription { get; set; }

        public Network()
        {
            Nodes = new List<Node>();
            Inputs = new List<Synapse>();
            Outputs = new List<Synapse>();
        }

        public IEnumerable<Synapse> GetAllSynapses()
        {
            List<Synapse> synapses = new List<Synapse>();
            synapses.AddRange(Inputs);
            synapses.AddRange(Outputs);
            foreach (Node node in Nodes)
            {
                if (node is Neuron)
                    synapses.AddRange((node as Neuron).Inputs);
                synapses.AddRange(node.Outputs);
            }

            return synapses.Distinct();
        }
        public int SynapseCount()
        {
            return GetAllSynapses().Count();
        }
        public void RemoveSynapse(Synapse synapse)
        {
            if (Inputs.Contains(synapse))
                Inputs.Remove(synapse);

            if (Outputs.Contains(synapse))
                Outputs.Remove(synapse);

            foreach (Node node in Nodes)
            {
                if (node is Neuron && (node as Neuron).Inputs.Contains(synapse))
                    (node as Neuron).RemoveInput(synapse);

                if (node.Outputs.Contains(synapse))
                    node.RemoveOutput(synapse);
            }
        }

        public ISpecimen DeepClone()
        {
            Network result = new Network();

            // Get all distinct synapses from the network
            List<Synapse> synapses = new List<Synapse>(GetAllSynapses());

            // Create synapses for result network with the same indexes
            List<Synapse> resultSynapses = new List<Synapse>();
            resultSynapses.AddRange(Enumerable.Range(1, synapses.Count).Select(i => new Synapse()));


            // Clone all neurons - use resultSynapses instead of synapses - with the same indices
            foreach (Node node in Nodes)
            {
                Node resultNode;
                if (node is Neuron)
                {
                    Neuron neuron = node as Neuron;
                    Neuron resultNeuron = new Neuron();
                    foreach (Synapse synapse in neuron.Inputs)
                    {
                        Synapse resultSynapse = resultSynapses[synapses.IndexOf(synapse)];
                        resultNeuron.AddInput(resultSynapse, neuron.InputMultiplier[neuron.Inputs.IndexOf(synapse)]);
                    }
                    resultNeuron.ActivationFunction = neuron.ActivationFunction;

                    resultNode = resultNeuron;
                }
                else if (node is Bias)
                {
                    resultNode = new Bias();
                }
                else throw new Exception("Unknown node type");

                foreach (Synapse synapse in node.Outputs)
                {
                    Synapse resultSynapse = resultSynapses[synapses.IndexOf(synapse)];
                    resultNode.AddOutput(resultSynapse);
                }

                result.Nodes.Add(resultNode);
            }

            // Clone references to input synapses - use resultSynapses instead of synapses - with the same indices
            foreach (Synapse synapse in Inputs)
            {
                Synapse resultSynapse = resultSynapses[synapses.IndexOf(synapse)];
                result.Inputs.Add(resultSynapse);
            }

            // Clone references to output synapses - use resultSynapses instead of synapses - with the same indices
            foreach (Synapse synapse in Outputs)
            {
                Synapse resultSynapse = resultSynapses[synapses.IndexOf(synapse)];
                result.Outputs.Add(resultSynapse);
            }

            return result;
        }

        public override string ToString()
        {
            return this.ToText();
        }

        public static Network CreateSimplest(int inputs, int outputs)
        {
            Network resultNetwork = new Network();

            // add inputs
            List<Synapse> inputSynapses = new List<Synapse>();
            inputSynapses.AddRange(Enumerable.Range(1, inputs).Select(i => new Synapse()));

            foreach (Synapse inputSynapse in inputSynapses)
            {
                // add neuron
                Neuron neuron = new Neuron();
                neuron.AddInput(inputSynapse, 1d);
                resultNetwork.Inputs.Add(inputSynapse);
                resultNetwork.Nodes.Add(neuron);
            }

            // add inputs
            List<Synapse> outputSynapses = new List<Synapse>();
            outputSynapses.AddRange(Enumerable.Range(1, outputs).Select(i => new Synapse()));

            foreach (Synapse outputSynapse in outputSynapses)
            {
                Neuron neuron = new Neuron();
                neuron.AddOutput(outputSynapse);
                resultNetwork.Outputs.Add(outputSynapse);
                resultNetwork.Nodes.Add(neuron);
            }

            return resultNetwork;
        }

    }
}
