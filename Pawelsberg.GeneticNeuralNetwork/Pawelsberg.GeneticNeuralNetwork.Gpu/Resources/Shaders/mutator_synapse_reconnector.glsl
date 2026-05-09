// Mirror of SynapseReconnectorNetworkMutator: pick two random nodes; with 50% prob
// (and both being neurons + first has inputs), move one input synapse from the first
// to the second. Otherwise (or as fallback), move one output synapse from first to
// second. Note CPU uses '> 0.5' for the input branch.

void mutateSynapseReconnector(uint s, inout uint rng) {
    int aN = nextActiveNodes(s);
    if (aN <= 0) return;
    int nodeIdx = rngInt(rng, aN);
    int newIdx = rngInt(rng, aN);
    if (nodeIdx == newIdx) return;

    bool tryInputBranch = rngFloat(rng) > 0.5;
    int nt = nextNodeType(s, nodeIdx);
    int newNt = nextNodeType(s, newIdx);

    if (tryInputBranch && nt == NODE_TYPE_NEURON && newNt == NODE_TYPE_NEURON
        && nextNodeInputCount(s, nodeIdx) > 0) {
        int newInCount = nextNodeInputCount(s, newIdx);
        if (newInCount >= MAX_INPUTS_PER_NODE) return;
        int inCount = nextNodeInputCount(s, nodeIdx);
        int i = rngInt(rng, inCount);
        int synIdx = nextNodeInputSynapse(s, nodeIdx, i);
        double mult = nextNodeInputMultiplier(s, nodeIdx, i);
        removeInputAt(s, nodeIdx, i);
        setNextNodeInputSynapse(s, newIdx, newInCount, synIdx);
        setNextNodeInputMultiplier(s, newIdx, newInCount, mult);
        setNextNodeInputCount(s, newIdx, newInCount + 1);
    } else if (nextNodeOutputCount(s, nodeIdx) > 0) {
        int newOutCount = nextNodeOutputCount(s, newIdx);
        if (newOutCount >= MAX_OUTPUTS_PER_NODE) return;
        int outCount = nextNodeOutputCount(s, nodeIdx);
        int o = rngInt(rng, outCount);
        int synIdx = nextNodeOutputSynapse(s, nodeIdx, o);
        removeOutputAt(s, nodeIdx, o);
        setNextNodeOutputSynapse(s, newIdx, newOutCount, synIdx);
        setNextNodeOutputCount(s, newIdx, newOutCount + 1);
    }
}
