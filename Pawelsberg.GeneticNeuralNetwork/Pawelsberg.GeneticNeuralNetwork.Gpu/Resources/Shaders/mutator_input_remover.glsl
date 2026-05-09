// Mirror of InputRemoverNetworkMutator: if more than 1 network input remains, pick
// a random one, locate the unique neuron that has it as an input, remove it from
// both the neuron and network.Inputs. The synapse becomes orphaned (no other refs in
// our model, since network inputs are only referenced from there + the neuron) so we
// free the slot.

void mutateInputRemover(uint s, inout uint rng) {
    int netInCount = nextNetworkInputCount(s);
    if (netInCount <= 1) return;

    int idx = rngInt(rng, netInCount);
    int synIdx = nextNetworkInputSynapse(s, idx);

    int aN = nextActiveNodes(s);
    for (int n = 0; n < aN; ++n) {
        if (nextNodeType(s, n) != NODE_TYPE_NEURON) continue;
        int inCount = nextNodeInputCount(s, n);
        for (int i = 0; i < inCount; ++i) {
            if (nextNodeInputSynapse(s, n, i) == synIdx) {
                removeInputAt(s, n, i);
                break;
            }
        }
    }

    for (int i = idx; i < netInCount - 1; ++i)
        setNextNetworkInputSynapse(s, i, nextNetworkInputSynapse(s, i + 1));
    setNextNetworkInputCount(s, netInCount - 1);

    setNextSynapseActive(s, synIdx, 0);
}
