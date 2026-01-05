using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics.Mutating;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.Mutating;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.Mutating;

public class NetworkMutators : Mutators<Network>
{
    public static NetworkMutators CreateNone()
    {
        NetworkMutators mutators = new NetworkMutators();
        mutators.Add(new NothingDoerMutator<Network>(), 1d);
        return mutators;
    }

    public static NetworkMutators CreateNormal(int maxNodes, int maxSynapses)
    {
        NetworkMutators mutators = new NetworkMutators();
        mutators.Add(new RandomNumberOfTimesMutator<Network>(new NeuronAdderNetworkMutator(maxNodes), 1, 3), 3d);
        mutators.Add(new RandomNumberOfTimesMutator<Network>(new BiasAdderNetworkMutator(maxNodes), 1, 3), 3d);
        mutators.Add(new RandomNumberOfTimesMutator<Network>(new NodeRemoverNetworkMutator(), 1, 3), 5d);
        mutators.Add(new RandomNumberOfTimesMutator<Network>(new NeuronModifierNetworkMutator(), 1, 3), 30d);
        mutators.Add(new RandomNumberOfTimesMutator<Network>(new SynapseAdderNetworkMutator(maxSynapses), 1, 3), 15d);
        mutators.Add(new RandomNumberOfTimesMutator<Network>(new SynapseRemoverNetworkMutator(), 1, 3), 5d);
        mutators.Add(new RandomNumberOfTimesMutator<Network>(new SynapseReconnectorNetworkMutator(), 1, 3), 15d);
        mutators.Add(new RandomNumberOfTimesMutator<Network>(new ActivationFunctionNetworkMutator(), 1, 3), 5d);
        mutators.Add(new MultipleTimesMutator<Network>(mutators, 5), 20d);
        mutators.Add(new RandomNumberOfTimesMutator<Network>(new NodeReordererNetworkMutator(), 1, 3), 2d);
        mutators.Add(new NeuronMergerNetworkMutator(), 5d);
        mutators.Add(new InputRemoverNetworkMutator(), 1d);
        mutators.Add(new OutputRemoverNetworkMutator(), 1d);
        mutators.Add(new SynapseReducerNetworkMutator(), 1d);
        mutators.Add(new NeuronReducerNetworkMutator(), 1d);
        mutators.Add(new RandomNumberOfTimesMutator<Network>(new TransparentNeuronAdderNetworkMutator(maxNodes, maxSynapses), 1, 2), 1d);
        mutators.Add(new NothingDoerMutator<Network>(), 1d);
        mutators.Add(new InputAdderNetworkMutator(maxSynapses), 0.1d);
        mutators.Add(new OutputAdderNetworkMutator(maxSynapses), 0.1d);
        return mutators;
    }

    public void UpdateParameters(int maxNodes, int maxSynapses, int propagations, TestCaseList testCaseList)
    {
        foreach (Mutator<Network> mutator in MutatorList)
            UpdateParameters(mutator, maxNodes, maxSynapses, propagations, testCaseList);
    }

    private void UpdateParameters(Mutator<Network> mutator, int maxNodes, int maxSynapses, int propagations, TestCaseList testCaseList)
    {
        if (mutator is IMaxNodesLimitedMutator maxNodesLimitedMutator)
            maxNodesLimitedMutator.MaxNodes = maxNodes;
        if (mutator is IMaxSynapsesLimitedMutator maxSynapsesLimitedMutator)
            maxSynapsesLimitedMutator.MaxSynapses = maxSynapses;
        if (mutator is ITestCaseDependentMutator testCaseDependentMutator)
        {
            testCaseDependentMutator.Propagations = propagations;
            testCaseDependentMutator.TestCaseList = testCaseList;
        }
        if (mutator is RandomNumberOfTimesMutator<Network> rnt)
            UpdateParameters(rnt.Mutator, maxNodes, maxSynapses, propagations, testCaseList);
        // Note: MultipleTimesMutator references this NetworkMutators instance (parentMutators),
        // so we don't recurse into it - the parent's MutatorList is already being iterated.
    }
}
