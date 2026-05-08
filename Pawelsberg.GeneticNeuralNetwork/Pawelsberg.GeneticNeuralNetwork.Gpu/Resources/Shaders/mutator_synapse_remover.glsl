// Mirror of SynapseRemoverNetworkMutator: pick a random node, gather its synapses
// (inputs + outputs), pick one randomly; if it is not the network's input or output
// boundary synapse, remove it from the network.

void mutateSynapseRemover(uint s, inout uint rng) {
    int aN = nextActiveNodes(s);
    if (aN <= 0) return;
    int n = rngInt(rng, aN);
    int nt = nextNodeType(s, n);

    int candidates[MAX_INPUTS_PER_NODE + MAX_OUTPUTS_PER_NODE];
    int candCount = 0;
    if (nt == NODE_TYPE_NEURON) {
        int inCount = nextNodeInputCount(s, n);
        for (int i = 0; i < inCount; ++i)
            candidates[candCount++] = nextNodeInputSynapse(s, n, i);
    }
    int outCount = nextNodeOutputCount(s, n);
    for (int o = 0; o < outCount; ++o)
        candidates[candCount++] = nextNodeOutputSynapse(s, n, o);

    if (candCount == 0) return;
    int picked = candidates[rngInt(rng, candCount)];
    if (isSynapseNetworkInput(s, picked) || isSynapseNetworkOutput(s, picked)) return;
    removeSynapseEverywhere(s, picked);
}
