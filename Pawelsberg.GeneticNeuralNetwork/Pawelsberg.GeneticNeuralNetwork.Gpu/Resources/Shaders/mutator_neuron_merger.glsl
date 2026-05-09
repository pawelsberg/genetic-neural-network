// Mirror of NeuronMergerNetworkMutator: pick two random nodes, if both are different
// neurons, move all inputs and outputs of the first into the second, then remove the
// first. If destination capacity is exceeded the mutator no-ops (CPU lacks this check
// but on GPU we have buffer-size limits).

void mutateNeuronMerger(uint s, inout uint rng) {
    int aN = nextActiveNodes(s);
    if (aN <= 0) return;
    int idx = rngInt(rng, aN);
    int newIdx = rngInt(rng, aN);
    if (idx == newIdx) return;
    if (nextNodeType(s, idx) != NODE_TYPE_NEURON) return;
    if (nextNodeType(s, newIdx) != NODE_TYPE_NEURON) return;

    int inCount = nextNodeInputCount(s, idx);
    int newInCount = nextNodeInputCount(s, newIdx);
    int outCount = nextNodeOutputCount(s, idx);
    int newOutCount = nextNodeOutputCount(s, newIdx);
    if (newInCount + inCount > MAX_INPUTS_PER_NODE) return;
    if (newOutCount + outCount > MAX_OUTPUTS_PER_NODE) return;

    for (int i = 0; i < inCount; ++i) {
        setNextNodeInputSynapse(s, newIdx, newInCount + i, nextNodeInputSynapse(s, idx, i));
        setNextNodeInputMultiplier(s, newIdx, newInCount + i, nextNodeInputMultiplier(s, idx, i));
    }
    setNextNodeInputCount(s, newIdx, newInCount + inCount);
    setNextNodeInputCount(s, idx, 0);

    for (int o = 0; o < outCount; ++o)
        setNextNodeOutputSynapse(s, newIdx, newOutCount + o, nextNodeOutputSynapse(s, idx, o));
    setNextNodeOutputCount(s, newIdx, newOutCount + outCount);
    setNextNodeOutputCount(s, idx, 0);

    compactNodeRemoveAt(s, idx);
}
