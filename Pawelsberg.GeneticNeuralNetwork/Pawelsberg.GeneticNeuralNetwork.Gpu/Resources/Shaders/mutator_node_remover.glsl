// Mirror of NodeRemoverNetworkMutator: pick a random node; remove only if active count > 1
// AND the node has no outputs AND (it's a Bias, or a Neuron with no inputs). Otherwise
// no-op. Compaction shifts higher-indexed nodes down.

void mutateNodeRemover(uint s, inout uint rng) {
    int aN = nextActiveNodes(s);
    if (aN <= 1) return;
    int idx = rngInt(rng, aN);
    int nt = nextNodeType(s, idx);
    int outCount = nextNodeOutputCount(s, idx);
    if (outCount != 0) return;
    if (nt == NODE_TYPE_NEURON && nextNodeInputCount(s, idx) != 0) return;
    if (nt != NODE_TYPE_NEURON && nt != NODE_TYPE_BIAS) return;

    compactNodeRemoveAt(s, idx);
}
