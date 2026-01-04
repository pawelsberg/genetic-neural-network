using System.Globalization;
using System.Text;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics.Mutating;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.Mutating;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting.Mutating.DiskStoring;

public static class NetworkMutatorsTextExtension
{
    // TODO Change codeded so that usage of a speciffic mutator is not needed here (maybe creating a child class of RandomNumberOfTimesMutator<Network>, MultipleTimesMutator<Network> and NothingDoerMutator<Network>)
    public static string ToText(this NetworkMutators mutators)
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < mutators.MutatorList.Count; i++)
        {
            double weight = mutators.MutatorWeights[i];
            Mutator<Network> mutator = mutators.MutatorList[i];
            string mutatorText = MutatorToText(mutator);
            sb.Append(weight.ToString(CultureInfo.InvariantCulture));
            sb.Append(' ');
            sb.AppendLine(mutatorText);
        }

        return sb.ToString();
    }

    private static string MutatorToText(Mutator<Network> mutator)
    {
        // Use interface if implemented
        if (mutator is INetworkMutatorTextConvertible convertible)
            return convertible.ToText();

        // Fallback for wrapper mutators
        return mutator switch
        {
            RandomNumberOfTimesMutator<Network> rnt => $"RandomNumberOfTimes({rnt.MinNumberOfTimes},{rnt.MaxNumberOfTimes},{MutatorToText(rnt.Mutator)})",
            MultipleTimesMutator<Network> mt => $"MultipleTimes({GetMultipleTimesCount(mt)},Self)",
            _ => throw new NotSupportedException($"Unknown mutator type: {mutator.GetType().Name}")
        };
    }

    private static int GetMultipleTimesCount(MultipleTimesMutator<Network> mutator)
    {
        var field = typeof(MultipleTimesMutator<Network>).GetField("_count", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return (int)field.GetValue(mutator);
    }

    public static NetworkMutators Parse(string text, int maxNodes, int maxSynapses, int propagations, TestCaseList testCaseList)
    {
        NetworkMutators mutators = new NetworkMutators();
        CodedText codedText = new CodedText(text);

        while (!codedText.EOT)
        {
            codedText.SkipWhiteCharacters();
            if (codedText.EOT)
                break;

            double weight = codedText.ReadDouble();
            codedText.SkipWhiteCharacters();

            Mutator<Network> mutator = ParseMutator(codedText, mutators, maxNodes, maxSynapses, propagations, testCaseList);
            mutators.Add(mutator, weight);
        }

        return mutators;
    }

    private static Mutator<Network> ParseMutator(CodedText codedText, NetworkMutators parentMutators, int maxNodes, int maxSynapses, int propagations, TestCaseList testCaseList)
    {
        codedText.SkipWhiteCharacters();

        if (codedText.TrySkip("RandomNumberOfTimes"))
        {
            string inner = codedText.ReadParenthesesContent();
            var parts = codedText.SplitTopLevel(inner);
            int min = int.Parse(parts[0], CultureInfo.InvariantCulture);
            int max = int.Parse(parts[1], CultureInfo.InvariantCulture);
            CodedText innerCodedText = new CodedText(parts[2]);
            Mutator<Network> innerMutator = ParseMutator(innerCodedText, parentMutators, maxNodes, maxSynapses, propagations, testCaseList);
            return new RandomNumberOfTimesMutator<Network>(innerMutator, min, max);
        }
        if (codedText.TrySkip("MultipleTimes"))
        {
            string inner = codedText.ReadParenthesesContent();
            var parts = codedText.SplitTopLevel(inner);
            int count = int.Parse(parts[0], CultureInfo.InvariantCulture);
            return new MultipleTimesMutator<Network>(parentMutators, count);
        }
        if (codedText.TrySkip("NothingDoer"))
            return new NothingDoerMutator<Network>();

        var mutator = TryParseConvertibleMutator(codedText, maxNodes, maxSynapses, propagations, testCaseList);
        if (mutator != null)
            return mutator;

        throw new FormatException($"Unknown mutator at character {codedText.Index}");
    }

    private static Mutator<Network>? TryParseConvertibleMutator(CodedText codedText, int maxNodes, int maxSynapses, int propagations, TestCaseList testCaseList)
    {
        var mutators = GetMutators(maxNodes, maxSynapses, propagations, testCaseList);

        foreach (var (name, mutator) in mutators)
        {
            if (codedText.TrySkip(name))
                return mutator();
        }

        return null;
    }

    private static Dictionary<string, Func<Mutator<Network>>> GetMutators(int maxNodes, int maxSynapses, int propagations, TestCaseList testCaseList)
    {
        return new Dictionary<string, Func<Mutator<Network>>>
        {
            { NeuronAdderNetworkMutator.TextName, () => new NeuronAdderNetworkMutator(maxNodes) },
            { BiasAdderNetworkMutator.TextName, () => new BiasAdderNetworkMutator(maxNodes) },
            { NodeRemoverNetworkMutator.TextName, () => new NodeRemoverNetworkMutator() },
            { NeuronModifierNetworkMutator.TextName, () => new NeuronModifierNetworkMutator() },
            { SynapseAdderNetworkMutator.TextName, () => new SynapseAdderNetworkMutator(maxSynapses) },
            { SynapseRemoverNetworkMutator.TextName, () => new SynapseRemoverNetworkMutator() },
            { SynapseReconnectorNetworkMutator.TextName, () => new SynapseReconnectorNetworkMutator() },
            { ActivationFunctionNetworkMutator.TextName, () => new ActivationFunctionNetworkMutator() },
            { NodeReordererNetworkMutator.TextName, () => new NodeReordererNetworkMutator() },
            { NeuronMergerNetworkMutator.TextName, () => new NeuronMergerNetworkMutator() },
            { InputRemoverNetworkMutator.TextName, () => new InputRemoverNetworkMutator() },
            { OutputRemoverNetworkMutator.TextName, () => new OutputRemoverNetworkMutator() },
            { SynapseReducerNetworkMutator.TextName, () => new SynapseReducerNetworkMutator() },
            { NeuronReducerNetworkMutator.TextName, () => new NeuronReducerNetworkMutator() },
            { TransparentNeuronAdderNetworkMutator.TextName, () => new TransparentNeuronAdderNetworkMutator(maxNodes, maxSynapses) },
            { InputAdderNetworkMutator.TextName, () => new InputAdderNetworkMutator(maxSynapses) },
            { OutputAdderNetworkMutator.TextName, () => new OutputAdderNetworkMutator(maxSynapses) },
            { BackPropagationNetworkMutator.TextName, () => new BackPropagationNetworkMutator(testCaseList, propagations) },
        };
    }
}
