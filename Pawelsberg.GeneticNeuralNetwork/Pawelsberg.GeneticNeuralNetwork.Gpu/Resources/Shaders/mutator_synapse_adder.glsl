// Mirror of SynapseAdderNetworkMutator: 50% continuous Randomize(0), 50% integer
// RandomInteger() for the new synapse's multiplier. Pick two random nodes, if end is
// a Neuron and there's a free synapse slot + per-node capacity, add a synapse.

void mutateSynapseAdder(uint s, inout uint rng) {
    int aN = nextActiveNodes(s);
    if (aN <= 0) return;
    if (countActiveNextSynapses(s) >= MUTATOR_MAX_SYNAPSES) return;
    int newSyn = findFreeSynapseSlot(s);
    if (newSyn < 0) return;

    bool integerVariant = rngFloat(rng) < 0.5;
    int startIdx = rngInt(rng, aN);
    int endIdx = rngInt(rng, aN);
    if (nextNodeType(s, endIdx) != NODE_TYPE_NEURON) return;

    int outCount = nextNodeOutputCount(s, startIdx);
    if (outCount >= MAX_OUTPUTS_PER_NODE) return;
    int inCount = nextNodeInputCount(s, endIdx);
    if (inCount >= MAX_INPUTS_PER_NODE) return;

    double multiplier = integerVariant ? double(rngRandomInteger(rng)) : double(rngRandomize(rng, 0.0));

    setNextSynapseActive(s, newSyn, 1);
    setNextNodeOutputSynapse(s, startIdx, outCount, newSyn);
    setNextNodeOutputCount(s, startIdx, outCount + 1);
    setNextNodeInputSynapse(s, endIdx, inCount, newSyn);
    setNextNodeInputMultiplier(s, endIdx, inCount, multiplier);
    setNextNodeInputCount(s, endIdx, inCount + 1);
}
