// Mirror of TransparentNeuronAdderNetworkMutator: insert a Linear neuron in the
// middle of an existing synapse. Two branches:
//   - synapse is NOT a network input: new neuron sits between the source node and
//     the original target neuron; new synapse goes source -> newNeuron, original
//     synapse becomes newNeuron -> target.
//   - synapse IS a network input: new neuron sits between the network input and the
//     target neuron; the original synapse becomes networkInput -> newNeuron and the
//     new synapse becomes newNeuron -> target (preserving target's multiplier).
// CPU: RandomGenerator.Random.Next(allSynapses.Count - 1) - we mirror the off-by-one.

void mutateTransparentNeuronAdder(uint s, inout uint rng) {
    int aN = nextActiveNodes(s);
    if (aN >= MUTATOR_MAX_NODES) return;
    if (aN >= MAX_NODES) return;
    int activeSyn = countActiveNextSynapses(s);
    if (activeSyn >= MUTATOR_MAX_SYNAPSES) return;
    if (activeSyn <= 1) return;
    int newSyn = findFreeSynapseSlot(s);
    if (newSyn < 0) return;

    int targetActiveIdx = rngInt(rng, activeSyn - 1);
    int targetSyn = findNthActiveSynapse(s, targetActiveIdx);
    if (targetSyn < 0) return;

    bool isNetworkInput = isSynapseNetworkInput(s, targetSyn);
    int newNodeIdx = aN;

    if (!isNetworkInput) {
        int inputNodeIdx = -1;
        int outputIdxInNode = -1;
        for (int n = 0; n < aN; ++n) {
            int nOutCount = nextNodeOutputCount(s, n);
            for (int o = 0; o < nOutCount; ++o) {
                if (nextNodeOutputSynapse(s, n, o) == targetSyn) {
                    inputNodeIdx = n;
                    outputIdxInNode = o;
                    break;
                }
            }
            if (inputNodeIdx >= 0) break;
        }
        if (inputNodeIdx < 0) return;

        setNextNodeOutputSynapse(s, inputNodeIdx, outputIdxInNode, newSyn);
        setNextSynapseActive(s, newSyn, 1);

        setNextNodeType(s, newNodeIdx, NODE_TYPE_NEURON);
        setNextNodeActivation(s, newNodeIdx, ACT_LINEAR);
        setNextNodeInputCount(s, newNodeIdx, 1);
        setNextNodeInputSynapse(s, newNodeIdx, 0, newSyn);
        setNextNodeInputMultiplier(s, newNodeIdx, 0, 1.0lf);
        setNextNodeOutputCount(s, newNodeIdx, 1);
        setNextNodeOutputSynapse(s, newNodeIdx, 0, targetSyn);
        setNextActiveNodes(s, aN + 1);
    } else {
        int outputNeuronIdx = -1;
        int inputIdxInNeuron = -1;
        for (int n = 0; n < aN; ++n) {
            if (nextNodeType(s, n) != NODE_TYPE_NEURON) continue;
            int nInCount = nextNodeInputCount(s, n);
            for (int i = 0; i < nInCount; ++i) {
                if (nextNodeInputSynapse(s, n, i) == targetSyn) {
                    outputNeuronIdx = n;
                    inputIdxInNeuron = i;
                    break;
                }
            }
            if (outputNeuronIdx >= 0) break;
        }
        if (outputNeuronIdx < 0) return;

        double multiplier = nextNodeInputMultiplier(s, outputNeuronIdx, inputIdxInNeuron);

        setNextNodeInputSynapse(s, outputNeuronIdx, inputIdxInNeuron, newSyn);
        setNextNodeInputMultiplier(s, outputNeuronIdx, inputIdxInNeuron, multiplier);
        setNextSynapseActive(s, newSyn, 1);

        setNextNodeType(s, newNodeIdx, NODE_TYPE_NEURON);
        setNextNodeActivation(s, newNodeIdx, ACT_LINEAR);
        setNextNodeInputCount(s, newNodeIdx, 1);
        setNextNodeInputSynapse(s, newNodeIdx, 0, targetSyn);
        setNextNodeInputMultiplier(s, newNodeIdx, 0, 1.0lf);
        setNextNodeOutputCount(s, newNodeIdx, 1);
        setNextNodeOutputSynapse(s, newNodeIdx, 0, newSyn);
        setNextActiveNodes(s, aN + 1);
    }
}
