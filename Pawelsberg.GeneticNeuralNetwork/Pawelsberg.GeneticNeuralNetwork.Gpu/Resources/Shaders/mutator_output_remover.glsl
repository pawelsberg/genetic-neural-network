// Mirror of OutputRemoverNetworkMutator: if more than 1 network output remains, pick
// a random one, locate the node that has it as an output (if any), remove from both
// the node and network.Outputs.

void mutateOutputRemover(uint s, inout uint rng) {
    int netOutCount = nextNetworkOutputCount(s);
    if (netOutCount <= 1) return;

    int idx = rngInt(rng, netOutCount);
    int synIdx = nextNetworkOutputSynapse(s, idx);

    int aN = nextActiveNodes(s);
    for (int n = 0; n < aN; ++n) {
        int outCount = nextNodeOutputCount(s, n);
        for (int o = 0; o < outCount; ++o) {
            if (nextNodeOutputSynapse(s, n, o) == synIdx) {
                removeOutputAt(s, n, o);
                break;
            }
        }
    }

    for (int i = idx; i < netOutCount - 1; ++i)
        setNextNetworkOutputSynapse(s, i, nextNetworkOutputSynapse(s, i + 1));
    setNextNetworkOutputCount(s, netOutCount - 1);

    setNextSynapseActive(s, synIdx, 0);
}
