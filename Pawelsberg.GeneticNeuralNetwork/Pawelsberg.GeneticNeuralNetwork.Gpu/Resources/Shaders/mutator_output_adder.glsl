// Mirror of OutputAdderNetworkMutator: if total synapses < MaxSynapses, pick a random
// node, create a new synapse as its output, and append to network.Outputs.

void mutateOutputAdder(uint s, inout uint rng) {
    if (countActiveNextSynapses(s) >= MUTATOR_MAX_SYNAPSES) return;
    int aN = nextActiveNodes(s);
    if (aN <= 0) return;
    int n = rngInt(rng, aN);

    int outCount = nextNodeOutputCount(s, n);
    if (outCount >= MAX_OUTPUTS_PER_NODE) return;
    int netOutCount = nextNetworkOutputCount(s);
    if (netOutCount >= MAX_NETWORK_OUTPUTS) return;
    int newSyn = findFreeSynapseSlot(s);
    if (newSyn < 0) return;

    setNextSynapseActive(s, newSyn, 1);
    setNextNodeOutputSynapse(s, n, outCount, newSyn);
    setNextNodeOutputCount(s, n, outCount + 1);
    setNextNetworkOutputSynapse(s, netOutCount, newSyn);
    setNextNetworkOutputCount(s, netOutCount + 1);
}
