using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting.DiskStoring;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating;

public class RunningContext
{
    public Dictionary<Synapse, Expression> SynapseExpressions { get; set; }
    public Dictionary<Synapse, Multiplier> MultiplierExpressions { get; set; }
    public Dictionary<Expression, Dictionary<Multiplier, Expression>> OutDerivativeOverMultiplier { get; set; }

    public RunningContext()
    {
        SynapseExpressions = new Dictionary<Synapse, Expression>();
        MultiplierExpressions = new Dictionary<Synapse, Multiplier>();
        OutDerivativeOverMultiplier = new Dictionary<Expression, Dictionary<Multiplier, Expression>>();
    }

    public void SetExpression(Synapse synapse, Expression expression)
    {
        SynapseExpressions[synapse] = expression;
    }
    public Expression GetExpression(Synapse synapse)
    {
        if (!SynapseExpressions.ContainsKey(synapse))
            throw new Exception("Synapse not found");
        return SynapseExpressions[synapse];
    }

    public void SetMultiplierExpression(Synapse inputSynapse, Multiplier multiplierExpression)
    {
        MultiplierExpressions[inputSynapse] = multiplierExpression;
    }
    private Multiplier GetMultiplierExpression(Synapse notOutSynapse)
    {
        if (!MultiplierExpressions.ContainsKey(notOutSynapse))
            throw new Exception("Synapse not found");
        return MultiplierExpressions[notOutSynapse];
    }

    private Expression GetOrCalcDerivativeOverMultiplier(Expression outExpression, Multiplier multiplier)
    {
        if (!OutDerivativeOverMultiplier.ContainsKey(outExpression))
            OutDerivativeOverMultiplier.Add(outExpression, new Dictionary<Multiplier, Expression>());
        if (!OutDerivativeOverMultiplier[outExpression].ContainsKey(multiplier))
            OutDerivativeOverMultiplier[outExpression].Add(multiplier, outExpression.DerivativeOver(multiplier));
        return OutDerivativeOverMultiplier[outExpression][multiplier];
    }

    public Dictionary<Synapse, double> CalcNotOutSynapsesCostInfluence(Network network, TestCase testCase)
    {
        Dictionary<Synapse, double> notOutSynapsesCostInfluence = new Dictionary<Synapse, double>();

        // zero all inputs (even not in test case)
        network.Inputs.Select(inSy => SynapseExpressions[inSy] as Input).ToList().ForEach(inp => inp.Value = 0);

        // apply input values from the test case
        for (int index = 0; index < testCase.Inputs.Count; index++)
        {
            Synapse inputSynapse = network.Inputs[index];
            int inputVal = testCase.Inputs[index];
            Input inputExpression = SynapseExpressions[inputSynapse] as Input;
            inputExpression.Value = inputVal;
        }

        // calculate all output values
        network.Outputs.Select(outSy => SynapseExpressions[outSy]).ToList().ForEach(outSy => outSy.Calc());


        foreach (Synapse notOutSynapse in network.GetAllSynapses().Where(s => !network.Outputs.Contains(s)))
        {
            Multiplier multiplierExpression = GetMultiplierExpression(notOutSynapse);
            double costInfluence = 0;
            foreach (Synapse outputSynapse in network.Outputs)
            {
                if (network.Outputs.IndexOf(outputSynapse) >= testCase.Outputs.Count)
                    continue;
                Expression outExpression = SynapseExpressions[outputSynapse];
                int expectedOutput = testCase.Outputs[network.Outputs.IndexOf(outputSynapse)];
                // calculate outExpression derivative over multiplierExpression

                Expression derivative = GetOrCalcDerivativeOverMultiplier(outExpression, multiplierExpression);

                derivative.Calc();

                costInfluence += 2 * derivative.Value * (expectedOutput - outExpression.Value);
            }

            notOutSynapsesCostInfluence.Add(notOutSynapse, costInfluence / network.Outputs.Count);
        }

        return notOutSynapsesCostInfluence;
    }

    public Dictionary<Synapse, double> CalcNotOutSynapsesCostInfluence(Network thisNetwork, TestCaseList testCaseList)
    {
        Dictionary<Synapse, double> totalNotOutSynapsesCostInfluence = new Dictionary<Synapse, double>();

        foreach (Synapse notOutSynapse in thisNetwork.GetAllSynapses().Where(s => !thisNetwork.Outputs.Contains(s)))
            totalNotOutSynapsesCostInfluence.Add(notOutSynapse, 0d);

        foreach (TestCase testCase in testCaseList.TestCases)
        {
            Dictionary<Synapse, double> notOutSynapsesCostInfluence = CalcNotOutSynapsesCostInfluence(thisNetwork, testCase);

            foreach (KeyValuePair<Synapse, double> notOutSynapseCostInfluence in notOutSynapsesCostInfluence)
                totalNotOutSynapsesCostInfluence[notOutSynapseCostInfluence.Key] = totalNotOutSynapsesCostInfluence[notOutSynapseCostInfluence.Key] + notOutSynapseCostInfluence.Value;
        }
        return totalNotOutSynapsesCostInfluence;
    }

    public void OptimiseExpressions()
    {
        foreach (Synapse synapse in SynapseExpressions.Keys.ToList())
            SynapseExpressions[synapse] = SynapseExpressions[synapse].Optimised();
    }
}
