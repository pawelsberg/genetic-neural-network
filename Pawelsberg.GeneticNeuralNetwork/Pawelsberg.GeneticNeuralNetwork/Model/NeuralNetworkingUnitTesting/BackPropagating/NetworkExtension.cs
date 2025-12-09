using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting.DiskStoring;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating;

public static class NetworkExtension
{
    private const double LearningRate = 0.05; // faster convergence while still bounded by scaling
    private const double InfluenceScale = 1d; // soft saturation threshold for normalised influence

    private static double ScaledStep(double influence)
    {
        // Soft-saturate update: near-linear for small influence, bounded for large.
        double ratio = influence / InfluenceScale;
        double denom = 1 + ratio * ratio;
        return LearningRate * (influence / denom);
    }

    private static Dictionary<Synapse, double> CalcInputSquaredAverage(Network network, TestCaseList testCaseList)
    {
        var averages = new Dictionary<Synapse, double>();

        for (int index = 0; index < network.Inputs.Count; index++)
        {
            var inputSynapse = network.Inputs[index];
            var squaredValues = testCaseList.TestCases
                .Where(tc => tc.Inputs.Count > index)
                .Select(tc => (double)tc.Inputs[index] * tc.Inputs[index]);

            double average = squaredValues.Any() ? squaredValues.Average() : 1d;
            averages[inputSynapse] = average <= 0 ? 1d : average;
        }

        return averages;
    }

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
        Dictionary<Synapse, double> inputSquaredAverage = CalcInputSquaredAverage(thisNetwork, testCaseList);

        int testCaseListCount = testCaseList.TestCases.Count;
        foreach (Neuron neuron in thisNetwork.Nodes.Where(n => n is Neuron))
        {
            foreach (Synapse inputSynapse in neuron.Inputs)
            {
                double averageInfluence = totalNotOutSynapsesCostInfluence[inputSynapse] / testCaseListCount;
                double inputNorm = inputSquaredAverage.TryGetValue(inputSynapse, out var avgSq) ? avgSq : 1d;
                double normalisedInfluence = averageInfluence / inputNorm;

                neuron.InputMultiplier[neuron.Inputs.IndexOf(inputSynapse)] += ScaledStep(normalisedInfluence);
            }
        }
    }
    public static void BackPropagation(this Network thisNetwork, TestCaseList testCaseList, int propagations, Synapse notOutSynapse)
    {
        RunningContext runningContext = thisNetwork.SafeRun(propagations);
        runningContext.OptimiseExpressions();

        double costInfluence = runningContext.CalcNotOutSynapsesCostInfluence(thisNetwork, testCaseList, notOutSynapse);
        Dictionary<Synapse, double> inputSquaredAverage = CalcInputSquaredAverage(thisNetwork, testCaseList);
        foreach (Neuron neuron in thisNetwork.Nodes.Where(n => n is Neuron))
        {
            foreach (Synapse inputSynapse in neuron.Inputs)
                if (inputSynapse == notOutSynapse)
                {
                    double inputNorm = inputSquaredAverage.TryGetValue(inputSynapse, out var avgSq) ? avgSq : 1d;
                    double normalisedInfluence = costInfluence / inputNorm;

                    neuron.InputMultiplier[neuron.Inputs.IndexOf(inputSynapse)] += ScaledStep(normalisedInfluence);
                }
        }
    }
}
