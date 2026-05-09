// Mirror of SynapseReducerNetworkMutator: scan all (source, destination-neuron) pairs.
// If a source node has multiple synapses going to the same destination neuron, sum
// their multipliers into one and remove the duplicate(s). Cleans up redundant
// connections introduced during evolution.

void mutateSynapseReducer(uint s, inout uint rng) {
    int aN = nextActiveNodes(s);
    for (int srcN = 0; srcN < aN; ++srcN) {
        int outCount = nextNodeOutputCount(s, srcN);
        for (int i = 0; i < outCount; ++i) {
            int synI = nextNodeOutputSynapse(s, srcN, i);

            int dstN = -1;
            int dstInputIdx = -1;
            for (int n = 0; n < aN; ++n) {
                if (nextNodeType(s, n) != NODE_TYPE_NEURON) continue;
                int dInCount = nextNodeInputCount(s, n);
                for (int k = 0; k < dInCount; ++k) {
                    if (nextNodeInputSynapse(s, n, k) == synI) {
                        dstN = n;
                        dstInputIdx = k;
                        break;
                    }
                }
                if (dstN >= 0) break;
            }
            if (dstN < 0) continue;

            for (int j = i + 1; j < outCount; ) {
                int synJ = nextNodeOutputSynapse(s, srcN, j);
                int dInCount = nextNodeInputCount(s, dstN);
                int dstInputIdxJ = -1;
                for (int k = 0; k < dInCount; ++k) {
                    if (nextNodeInputSynapse(s, dstN, k) == synJ) {
                        dstInputIdxJ = k;
                        break;
                    }
                }
                if (dstInputIdxJ < 0) {
                    ++j;
                    continue;
                }

                double mI = nextNodeInputMultiplier(s, dstN, dstInputIdx);
                double mJ = nextNodeInputMultiplier(s, dstN, dstInputIdxJ);
                setNextNodeInputMultiplier(s, dstN, dstInputIdx, mI + mJ);

                removeInputAt(s, dstN, dstInputIdxJ);
                if (dstInputIdxJ < dstInputIdx) dstInputIdx--;

                removeOutputAt(s, srcN, j);
                outCount--;

                setNextSynapseActive(s, synJ, 0);
            }
        }
    }
}
