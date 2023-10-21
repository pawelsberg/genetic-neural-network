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
        network
            .GetAllSynapses()
            .Select(inSy => SynapseExpressions[inSy])
            .SelectMany(syEx=>syEx.GetExpressionsRecursive())
            .Select(ex=>ex as Input)
            .Where(inEx=>inEx is not null)
            .ToList()
            .ForEach(inEx => inEx.Value = 0);

        // apply input values from the test case
        for (int index = 0; index < testCase.Inputs.Count; index++)
        {
            Synapse inputSynapse = network.Inputs[index];
            int inputVal = testCase.Inputs[index];
            List<Input> inputs = SynapseExpressions
                .Values
                .SelectMany(synEx => synEx.GetExpressionsRecursive())
                .Where(input => input is Input)
                .Cast<Input>()
                .Where(input => input.InputSynapse == inputSynapse)
                .ToList();

            foreach (var input in inputs)
                input.Value = inputVal;
        }

        // calculate all output values
        network.Outputs.Select(outSy => SynapseExpressions[outSy]).ToList().ForEach(outSy => outSy.CalcValue());

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
                
                // apply input values from the test case
                for (int index = 0; index < testCase.Inputs.Count; index++)
                {
                    Synapse inputSynapse = network.Inputs[index];
                    int inputVal = testCase.Inputs[index];
                    List<Input> inputs = derivative.GetExpressionsRecursive()
                        .Where(input => input is Input)
                        .Cast<Input>()
                        .Where(input => input.InputSynapse == inputSynapse)
                        .ToList();

                    foreach (var input in inputs)
                        input.Value = inputVal;
                }

                derivative.CalcValue();

                costInfluence += derivative.Value * (expectedOutput - outExpression.Value);
            }

            notOutSynapsesCostInfluence.Add(notOutSynapse, costInfluence / network.Outputs.Count);
        }

        return notOutSynapsesCostInfluence;
    }

    public double CalcNotOutSynapsesCostInfluence(Network network, TestCase testCase, Synapse notOutSynapse)
    {

        // zero all inputs (even not in test case)
        network
            .GetAllSynapses()
            .Select(inSy => SynapseExpressions[inSy])
            .SelectMany(syEx => syEx.GetExpressionsRecursive())
            .Select(ex => ex as Input)
            .Where(inEx => inEx is not null)
            .ToList()
            .ForEach(inEx => inEx.Value = 0);

        // apply input values from the test case
        for (int index = 0; index < testCase.Inputs.Count; index++)
        {
            Synapse inputSynapse = network.Inputs[index];
            int inputVal = testCase.Inputs[index];
            List<Input> inputs = SynapseExpressions
                .Values
                .SelectMany(synEx => synEx.GetExpressionsRecursive())
                .Where(input => input is Input)
                .Cast<Input>()
                .Where(input => input.InputSynapse == inputSynapse)
                .ToList();

            foreach (var input in inputs)
                input.Value = inputVal;
        }

        // calculate all output values
        network.Outputs.Select(outSy => SynapseExpressions[outSy]).ToList().ForEach(outSy => outSy.CalcValue());


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
            derivative = derivative.Optimised();
            // apply input values from the test case
            for (int index = 0; index < testCase.Inputs.Count; index++)
            {
                Synapse inputSynapse = network.Inputs[index];
                int inputVal = testCase.Inputs[index];
                List<Input> inputs = derivative.GetExpressionsRecursive()
                    .Where(input => input is Input)
                    .Cast<Input>()
                    .Where(input => input.InputSynapse == inputSynapse)
                    .ToList();

                foreach (var input in inputs)
                    input.Value = inputVal;
            }

            derivative.CalcValue();

            costInfluence += derivative.Value * (expectedOutput - outExpression.Value);
        }

        return costInfluence / network.Outputs.Count;
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

    public double CalcNotOutSynapsesCostInfluence(Network thisNetwork, TestCaseList testCaseList, Synapse notOutSynapse)
    {
        double costInfluence = 0;
        foreach (TestCase testCase in testCaseList.TestCases)
        {
            costInfluence += CalcNotOutSynapsesCostInfluence(thisNetwork, testCase, notOutSynapse);
        }
        return costInfluence / testCaseList.TestCases.Count;
    }


    public void OptimiseExpressions()
    {
        foreach (Synapse synapse in SynapseExpressions.Keys.ToList())
            SynapseExpressions[synapse] = SynapseExpressions[synapse].Optimised();
    }
}
