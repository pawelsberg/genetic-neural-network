// Mirror of NeuronReducerNetworkMutator: iteratively remove "transparent" neurons -
// Neurons with exactly 1 input and 1 output, where the input synapse is not a network
// input, the output synapse is not a network output, and the two synapses differ.
// The downstream neuron's input synapse is replaced with the transparent neuron's
// input synapse, multiplying the multiplier by the transparent's input multiplier.

void mutateNeuronReducer(uint s, inout uint rng) {
    int safety = MAX_NODES * 2;
    bool changed = true;
    while (changed && safety > 0) {
        --safety;
        changed = false;
        int aN = nextActiveNodes(s);
        for (int n = 0; n < aN; ++n) {
            if (nextNodeType(s, n) != NODE_TYPE_NEURON) continue;
            if (nextNodeInputCount(s, n) != 1) continue;
            if (nextNodeOutputCount(s, n) != 1) continue;
            int inSyn = nextNodeInputSynapse(s, n, 0);
            int outSyn = nextNodeOutputSynapse(s, n, 0);
            if (inSyn == outSyn) continue;
            if (isSynapseNetworkInput(s, inSyn)) continue;
            if (isSynapseNetworkOutput(s, outSyn)) continue;

            double thisMult = nextNodeInputMultiplier(s, n, 0);

            int outNeuronIdx = -1;
            int outNeuronSynIdx = -1;
            for (int m = 0; m < aN; ++m) {
                if (nextNodeType(s, m) != NODE_TYPE_NEURON) continue;
                int mInCount = nextNodeInputCount(s, m);
                for (int k = 0; k < mInCount; ++k) {
                    if (nextNodeInputSynapse(s, m, k) == outSyn) {
                        outNeuronIdx = m;
                        outNeuronSynIdx = k;
                        break;
                    }
                }
                if (outNeuronIdx >= 0) break;
            }
            if (outNeuronIdx < 0) continue;

            double oldMult = nextNodeInputMultiplier(s, outNeuronIdx, outNeuronSynIdx);
            setNextNodeInputSynapse(s, outNeuronIdx, outNeuronSynIdx, inSyn);
            setNextNodeInputMultiplier(s, outNeuronIdx, outNeuronSynIdx, oldMult * thisMult);

            setNextSynapseActive(s, outSyn, 0);
            compactNodeRemoveAt(s, n);

            changed = true;
            break;
        }
    }
}
