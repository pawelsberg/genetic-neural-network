using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating;

public static class NeuronExtension
{
    public static void Propagate(this Neuron thisNeuron, RunningContext runningContext)
    {
        SumOperation sumExpression = new SumOperation();

        // calculate output potential
        Dictionary<Synapse, Expression> synapseExpressions = runningContext.SynapseExpressions;
        for (int inputIndex = 0; inputIndex < thisNeuron.Inputs.Count; inputIndex++)
        {
            Synapse inputSynapse = thisNeuron.Inputs[inputIndex];
            MultiplyOperation multiplyOperation = new MultiplyOperation();
            multiplyOperation.Expressions.Add(synapseExpressions[inputSynapse]);
            Multiplier multiplierExpression = new Multiplier() { Synapse = inputSynapse, Value = thisNeuron.InputMultiplier[inputIndex] };
            runningContext.SetMultiplierExpression(inputSynapse, multiplierExpression);
            multiplyOperation.Expressions.Add(multiplierExpression);
            sumExpression.Expressions.Add(multiplyOperation);
        }

        ActivationFunctionOperation outputExpression = new ActivationFunctionOperation() { ActivationFunction = thisNeuron.ActivationFunction, Expression = sumExpression };

        foreach (Synapse outputSynapse in thisNeuron.Outputs)
        {
            // copy potential to output synapse
            runningContext.SetExpression(outputSynapse, outputExpression);
        }
    }
}
