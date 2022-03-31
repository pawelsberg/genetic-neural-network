using System.Text;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking
{
    public static class NetworkTextExtension
    {
        public static string ToText(this Network net)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (Synapse inputSynapse in net.Inputs)
            {
                Neuron destinationNeuron = net.Nodes.OfType<Neuron>().First(neuron => neuron.Inputs.Contains(inputSynapse));
                stringBuilder.AppendFormat("Input {0} * {1:F13}", net.Inputs.IndexOf(inputSynapse), destinationNeuron.InputMultiplier[destinationNeuron.Inputs.IndexOf(inputSynapse)]);
                stringBuilder.Append(" -> ");
                stringBuilder.AppendFormat("Neuron({0}) {1}", destinationNeuron.ActivationFunction.ToLetterCode(), net.Nodes.IndexOf(destinationNeuron));
                stringBuilder.AppendLine();
            }
            foreach (Synapse innerSynapse in net.GetAllSynapses().Except(net.Inputs).Except(net.Outputs))
            {
                Node sourceNode = net.Nodes.First(node => node.Outputs.Contains(innerSynapse));
                Neuron destinatioNeuron = net.Nodes.OfType<Neuron>().First(neuron => neuron.Inputs.Contains(innerSynapse));

                if (sourceNode is Neuron)
                    stringBuilder.AppendFormat("Neuron({0}) {1}", (sourceNode as Neuron).ActivationFunction.ToLetterCode(), net.Nodes.IndexOf(sourceNode));
                else if (sourceNode is Bias)
                    stringBuilder.AppendFormat("Bias {0}", net.Nodes.IndexOf(sourceNode));
                else
                    throw new Exception("Unknown source node type");

                stringBuilder.AppendFormat(" * {0:F13}", destinatioNeuron.InputMultiplier[destinatioNeuron.Inputs.IndexOf(innerSynapse)]);
                stringBuilder.Append(" -> ");
                stringBuilder.AppendFormat("Neuron({0}) {1}", destinatioNeuron.ActivationFunction.ToLetterCode(), net.Nodes.IndexOf(destinatioNeuron));
                stringBuilder.AppendLine();
            }

            foreach (Synapse outputSynapse in net.Outputs)
            {
                Node sourceNode = net.Nodes.First(node => node.Outputs.Contains(outputSynapse));
                if (sourceNode is Neuron)
                    stringBuilder.AppendFormat("Neuron({0}) {1}", (sourceNode as Neuron).ActivationFunction.ToLetterCode(), net.Nodes.IndexOf(sourceNode));
                else if (sourceNode is Bias)
                    stringBuilder.AppendFormat("Bias {0}", net.Nodes.IndexOf(sourceNode));
                else
                    throw new Exception("Unknown source node type");
                stringBuilder.Append(" -> ");
                stringBuilder.AppendFormat("Output {0}", net.Outputs.IndexOf(outputSynapse));
                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }
        public static Network Parse(string text)
        {
            Network result = new Network();

            Dictionary<int, Synapse> inputSynapses = new Dictionary<int, Synapse>();
            Dictionary<int, Node> nodes = new Dictionary<int, Node>();
            Dictionary<int, Synapse> outputSynapses = new Dictionary<int, Synapse>();
            // 
            CodedText codedText = new CodedText(text);
            while (!codedText.EOT)
            {
                codedText.SkipWhiteCharacters();
                // read neuron or input
                Synapse inputSynapse = null;
                Node inputNode = null;
                // if it is an input
                if (codedText.TrySkip("Input"))
                {
                    inputSynapse = new Synapse();
                    codedText.SkipWhiteCharacters();
                    int inputIndex = codedText.ReadInt();
                    inputSynapses.Add(inputIndex, inputSynapse);
                }
                else if (codedText.TrySkip("Neuron"))
                {
                    Neuron inputNeuron = new Neuron();
                    if (codedText.TrySkip("("))
                    {
                        string activationFunctionCode = codedText.ReadString(1);
                        if (activationFunctionCode == "A")
                            // for backward compatibility A means threshold
                            inputNeuron.ActivationFunction = ActivationFunction.Threshold;
                        else
                            inputNeuron.ActivationFunction = ActivationFunctionExtension.FromLetterCode(activationFunctionCode);


                        codedText.Skip(")");
                    }
                    else
                    {
                        // for backward compatibility default was linear
                        inputNeuron.ActivationFunction = ActivationFunction.Linear;
                    }

                    codedText.SkipWhiteCharacters();
                    int inputNeuronIndex = codedText.ReadInt();
                    if (!nodes.ContainsKey(inputNeuronIndex))
                        nodes.Add(inputNeuronIndex, inputNeuron);
                    else if (nodes[inputNeuronIndex] is Neuron)
                        inputNeuron = nodes[inputNeuronIndex] as Neuron;
                    else
                        throw new FormatException(string.Format("Index {0} was previously used for a Bias. Cannot use it for a Neuron. (character number {1})", inputNeuronIndex, codedText.Index));
                    inputNode = inputNeuron;
                }
                else if (codedText.TrySkip("Bias"))
                {
                    Bias inputBias = new Bias();

                    codedText.SkipWhiteCharacters();
                    int inputBiasIndex = codedText.ReadInt();
                    if (!nodes.ContainsKey(inputBiasIndex))
                        nodes.Add(inputBiasIndex, inputBias);
                    else if (nodes[inputBiasIndex] is Bias)
                        inputBias = nodes[inputBiasIndex] as Bias;
                    else
                        throw new FormatException(string.Format("Index {0} was previously used for a Neuron. Cannot use it for a Bias. (character number {1})", inputBiasIndex, codedText.Index));
                    inputNode = inputBias;

                }
                else
                    throw new FormatException(string.Format("Expected Neuron, Input or Bias (character number {0})", codedText.Index));
                // skip white characters - after Input id or neuron id
                codedText.SkipWhiteCharacters();

                double? multiplier = null;
                if (codedText.TrySkip("*"))
                {
                    codedText.SkipWhiteCharacters();
                    multiplier = codedText.ReadDouble();

                }

                codedText.SkipWhiteCharacters();

                // read arrow before neuron/output
                if (!codedText.TrySkip("->"))
                    throw new FormatException(string.Format("Expected -> (character number {0})", codedText.Index));
                codedText.SkipWhiteCharacters();


                Neuron outputNeuron = null;
                Synapse outputSynapse = null;

                // reading neuron or output
                if (multiplier.HasValue)
                    if (codedText.TrySkip("Neuron"))
                    {
                        outputNeuron = new Neuron();
                        if (codedText.TrySkip("("))
                        {
                            string activationFunctionCode = codedText.ReadString(1);
                            if (activationFunctionCode == "A")
                                // for backward compatibility A means threshold
                                outputNeuron.ActivationFunction = ActivationFunction.Threshold;
                            else
                                outputNeuron.ActivationFunction = ActivationFunctionExtension.FromLetterCode(activationFunctionCode);
                            codedText.Skip(")");
                        }
                        else
                        {
                            // for backward compatibility default was linear
                            outputNeuron.ActivationFunction = ActivationFunction.Linear;
                        }

                        codedText.SkipWhiteCharacters();
                        int outputNeuronIndex = codedText.ReadInt();
                        if (!nodes.ContainsKey(outputNeuronIndex))
                            nodes.Add(outputNeuronIndex, outputNeuron);
                        else if (nodes[outputNeuronIndex] is Neuron)
                            outputNeuron = nodes[outputNeuronIndex] as Neuron;
                        else
                            throw new FormatException(
                                string.Format("Index {0} was previously used for a Bias. Cannot use it as an Output. (character number {1})",
                                    outputNeuronIndex, codedText.Index));
                    }
                    else throw new FormatException(string.Format("Expected Neuron because there was a multiplier. (character number {0})", codedText.Index));
                else
                {
                    if (codedText.TrySkip("Output"))
                    {
                        outputSynapse = new Synapse();
                        codedText.SkipWhiteCharacters();
                        int outputIndex = codedText.ReadInt();
                        outputSynapses.Add(outputIndex, outputSynapse);
                    }
                    else throw new FormatException(string.Format("Expected Output because there was no multiplier. (character number {0})", codedText.Index));
                }

                codedText.SkipWhiteCharacters();

                // adding synapses to the neurons
                if (inputSynapse != null && multiplier != null && outputNeuron != null)
                {
                    outputNeuron.AddInput(inputSynapse, multiplier.Value);
                }
                else if (inputNode != null && multiplier != null && outputNeuron != null)
                {
                    Synapse synapse = new Synapse();
                    outputNeuron.AddInput(synapse, multiplier.Value);
                    inputNode.AddOutput(synapse);
                }
                else if (inputNode != null && multiplier == null && outputSynapse != null)
                {
                    inputNode.AddOutput(outputSynapse);
                }
                else throw new FormatException(string.Format("Syntax error (character number {0})", codedText.Index));
            }

            // adding all to the network
            // TODO - fix - add dummy synapse if there is a gap
            for (int intputIndex = 0; intputIndex < inputSynapses.Count; intputIndex++)
                result.Inputs.Add(inputSynapses[intputIndex]);

            for (int nodeIndex = 0; nodeIndex <= nodes.Keys.Max(); nodeIndex++)
            {
                Node node = nodes.ContainsKey(nodeIndex) ? nodes[nodeIndex] : new Neuron();
                result.Nodes.Add(node);
            }

            // TODO - fix - add dummy synapse if there is a gap
            for (int outputIndex = 0; outputIndex < outputSynapses.Count; outputIndex++)
                result.Outputs.Add(outputSynapses[outputIndex]);

            return result;
        }

    }
}
