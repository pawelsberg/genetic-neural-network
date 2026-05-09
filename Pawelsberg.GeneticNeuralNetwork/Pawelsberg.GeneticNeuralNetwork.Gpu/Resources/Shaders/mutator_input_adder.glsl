// Mirror of InputAdderNetworkMutator: if total synapses < MaxSynapses, pick a random
// node; if it's a Neuron, create a new synapse, add it as an input to the neuron with
// Randomize(0) multiplier, and append to network.Inputs.

void mutateInputAdder(uint s, inout uint rng) {
    if (countActiveNextSynapses(s) >= MUTATOR_MAX_SYNAPSES) return;
    int aN = nextActiveNodes(s);
    if (aN <= 0) return;
    int n = rngInt(rng, aN);
    if (nextNodeType(s, n) != NODE_TYPE_NEURON) return;

    int inCount = nextNodeInputCount(s, n);
    if (inCount >= MAX_INPUTS_PER_NODE) return;
    int netInCount = nextNetworkInputCount(s);
    if (netInCount >= MAX_NETWORK_INPUTS) return;
    int newSyn = findFreeSynapseSlot(s);
    if (newSyn < 0) return;

    setNextSynapseActive(s, newSyn, 1);
    setNextNodeInputSynapse(s, n, inCount, newSyn);
    setNextNodeInputMultiplier(s, n, inCount, double(rngRandomize(rng, 0.0)));
    setNextNodeInputCount(s, n, inCount + 1);
    setNextNetworkInputSynapse(s, netInCount, newSyn);
    setNextNetworkInputCount(s, netInCount + 1);
}
