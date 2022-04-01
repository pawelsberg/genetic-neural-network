using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting.DiskStoring;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating;

public static class NetworkExtension
{
    public static void SafeRun(this Network thisNetwork, RunningContext runningContext, int propagations)
    {
        for (int networkPropagation = 0; networkPropagation < propagations; networkPropagation++)
            foreach (Node node in thisNetwork.Nodes)
            {
                // put it in NodeExtension?
                if (node is Neuron)
                    ((Neuron)node).Propagate(runningContext);
                else if (node is Bias)
                    ((Bias)node).Propagate(runningContext);
            }
    }

    public static RunningContext SafeRun(this Network thisNetwork, int propagations)
    {
        RunningContext runningContext = new RunningContext();
        // zero all expressions at the beginning
        foreach (Synapse synapse in thisNetwork.GetAllSynapses())
        {
            runningContext.SynapseExpressions.Add(synapse, new Constant { Value = 0 });
        }
        // set input expressions
        foreach (Synapse inputSynapse in thisNetwork.Inputs)
            runningContext.SetExpression(inputSynapse, new Input() { InputSynapse = inputSynapse });

        thisNetwork.SafeRun(runningContext, propagations);
        return runningContext;
    }

    public static void BackPropagation(this Network thisNetwork, TestCaseList testCaseList, int propagations)
    {
        //return;
        RunningContext runningContext = thisNetwork.SafeRun(propagations);
        runningContext.OptimiseExpressions();

        Dictionary<Synapse, double> totalNotOutSynapsesCostInfluence = runningContext.CalcNotOutSynapsesCostInfluence(thisNetwork, testCaseList);

        int testCaseListCount = testCaseList.TestCases.Count;
        foreach (Neuron neuron in thisNetwork.Nodes.Where(n => n is Neuron))
        {
            foreach (Synapse inputSynapse in neuron.Inputs)
            {
                neuron.InputMultiplier[neuron.Inputs.IndexOf(inputSynapse)] += 0.0000002 * totalNotOutSynapsesCostInfluence[inputSynapse] / testCaseListCount;
            }
        }
    }


}
