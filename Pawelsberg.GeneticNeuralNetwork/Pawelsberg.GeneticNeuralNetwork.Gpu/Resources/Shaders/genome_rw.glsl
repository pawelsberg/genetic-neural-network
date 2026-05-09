// Read/write accessors for the "next generation" genome buffer + structural helpers.
// Used only by the mutate compute shader.

int nextActiveNodes(uint s) { return nextGenomeI[specOffsetI(s) + OFF_ACTIVE_NODES]; }
void setNextActiveNodes(uint s, int v) { nextGenomeI[specOffsetI(s) + OFF_ACTIVE_NODES] = v; }
int nextNetworkInputCount(uint s) { return nextGenomeI[specOffsetI(s) + OFF_NETWORK_INPUT_COUNT]; }
int nextNetworkOutputCount(uint s) { return nextGenomeI[specOffsetI(s) + OFF_NETWORK_OUTPUT_COUNT]; }
int nextNodeType(uint s, int n) { return nextGenomeI[specOffsetI(s) + OFF_NODE_TYPE + n]; }
void setNextNodeType(uint s, int n, int v) { nextGenomeI[specOffsetI(s) + OFF_NODE_TYPE + n] = v; }
int nextNodeActivation(uint s, int n) { return nextGenomeI[specOffsetI(s) + OFF_NODE_ACTIVATION + n]; }
void setNextNodeActivation(uint s, int n, int v) { nextGenomeI[specOffsetI(s) + OFF_NODE_ACTIVATION + n] = v; }
int nextNodeInputCount(uint s, int n) { return nextGenomeI[specOffsetI(s) + OFF_NODE_INPUT_COUNT + n]; }
void setNextNodeInputCount(uint s, int n, int v) { nextGenomeI[specOffsetI(s) + OFF_NODE_INPUT_COUNT + n] = v; }
int nextNodeOutputCount(uint s, int n) { return nextGenomeI[specOffsetI(s) + OFF_NODE_OUTPUT_COUNT + n]; }
void setNextNodeOutputCount(uint s, int n, int v) { nextGenomeI[specOffsetI(s) + OFF_NODE_OUTPUT_COUNT + n] = v; }
int nextNodeInputSynapse(uint s, int n, int i) { return nextGenomeI[specOffsetI(s) + OFF_NODE_INPUT_SYNAPSE + n * MAX_INPUTS_PER_NODE + i]; }
void setNextNodeInputSynapse(uint s, int n, int i, int v) { nextGenomeI[specOffsetI(s) + OFF_NODE_INPUT_SYNAPSE + n * MAX_INPUTS_PER_NODE + i] = v; }
int nextNodeOutputSynapse(uint s, int n, int o) { return nextGenomeI[specOffsetI(s) + OFF_NODE_OUTPUT_SYNAPSE + n * MAX_OUTPUTS_PER_NODE + o]; }
void setNextNodeOutputSynapse(uint s, int n, int o, int v) { nextGenomeI[specOffsetI(s) + OFF_NODE_OUTPUT_SYNAPSE + n * MAX_OUTPUTS_PER_NODE + o] = v; }
double nextNodeInputMultiplier(uint s, int n, int i) { return nextGenomeM[specOffsetM(s) + n * MAX_INPUTS_PER_NODE + i]; }
void setNextNodeInputMultiplier(uint s, int n, int i, double v) { nextGenomeM[specOffsetM(s) + n * MAX_INPUTS_PER_NODE + i] = v; }
int nextNetworkInputSynapse(uint s, int i) { return nextGenomeI[specOffsetI(s) + OFF_NETWORK_INPUT_SYNAPSE + i]; }
void setNextNetworkInputSynapse(uint s, int i, int v) { nextGenomeI[specOffsetI(s) + OFF_NETWORK_INPUT_SYNAPSE + i] = v; }
void setNextNetworkInputCount(uint s, int v) { nextGenomeI[specOffsetI(s) + OFF_NETWORK_INPUT_COUNT] = v; }
int nextNetworkOutputSynapse(uint s, int i) { return nextGenomeI[specOffsetI(s) + OFF_NETWORK_OUTPUT_SYNAPSE + i]; }
void setNextNetworkOutputSynapse(uint s, int i, int v) { nextGenomeI[specOffsetI(s) + OFF_NETWORK_OUTPUT_SYNAPSE + i] = v; }
void setNextNetworkOutputCount(uint s, int v) { nextGenomeI[specOffsetI(s) + OFF_NETWORK_OUTPUT_COUNT] = v; }
int nextSynapseActive(uint s, int idx) { return nextGenomeI[specOffsetI(s) + OFF_SYNAPSE_ACTIVE + idx]; }
void setNextSynapseActive(uint s, int idx, int v) { nextGenomeI[specOffsetI(s) + OFF_SYNAPSE_ACTIVE + idx] = v; }

// Find the targetActiveIdx-th active synapse slot, or -1 if not found.
int findNthActiveSynapse(uint s, int targetIdx) {
    int counter = 0;
    for (int i = 0; i < MAX_SYNAPSES; ++i) {
        if (nextSynapseActive(s, i) != 0) {
            if (counter == targetIdx) return i;
            counter++;
        }
    }
    return -1;
}

// Remove input #i from neuron n by shifting subsequent inputs down.
void removeInputAt(uint s, int n, int i) {
    int inCount = nextNodeInputCount(s, n);
    for (int j = i; j < inCount - 1; ++j) {
        setNextNodeInputSynapse(s, n, j, nextNodeInputSynapse(s, n, j + 1));
        setNextNodeInputMultiplier(s, n, j, nextNodeInputMultiplier(s, n, j + 1));
    }
    setNextNodeInputCount(s, n, inCount - 1);
}

// Remove output #o from node n by shifting subsequent outputs down.
void removeOutputAt(uint s, int n, int o) {
    int outCount = nextNodeOutputCount(s, n);
    for (int j = o; j < outCount - 1; ++j)
        setNextNodeOutputSynapse(s, n, j, nextNodeOutputSynapse(s, n, j + 1));
    setNextNodeOutputCount(s, n, outCount - 1);
}

void cloneParentIntoChild(uint childIdx, uint parentIdx) {
    int srcI = specOffsetI(parentIdx);
    int dstI = specOffsetI(childIdx);
    for (int i = 0; i < INT_STRIDE; ++i)
        nextGenomeI[dstI + i] = genomeI[srcI + i];
    int srcM = specOffsetM(parentIdx);
    int dstM = specOffsetM(childIdx);
    for (int i = 0; i < MULT_STRIDE; ++i)
        nextGenomeM[dstM + i] = genomeM[srcM + i];
}

int findFreeSynapseSlot(uint s) {
    for (int i = 0; i < MAX_SYNAPSES; ++i)
        if (nextSynapseActive(s, i) == 0) return i;
    return -1;
}

int countActiveNextSynapses(uint s) {
    int count = 0;
    for (int i = 0; i < MAX_SYNAPSES; ++i)
        if (nextSynapseActive(s, i) != 0) count++;
    return count;
}

bool isSynapseNetworkInput(uint s, int synIdx) {
    int c = nextNetworkInputCount(s);
    for (int i = 0; i < c; ++i)
        if (nextNetworkInputSynapse(s, i) == synIdx) return true;
    return false;
}

bool isSynapseNetworkOutput(uint s, int synIdx) {
    int c = nextNetworkOutputCount(s);
    for (int i = 0; i < c; ++i)
        if (nextNetworkOutputSynapse(s, i) == synIdx) return true;
    return false;
}

void removeSynapseFromNode(uint s, int n, int synIdx) {
    int nt = nextNodeType(s, n);
    if (nt == NODE_TYPE_NEURON) {
        int inCount = nextNodeInputCount(s, n);
        int newCount = 0;
        for (int i = 0; i < inCount; ++i) {
            int existing = nextNodeInputSynapse(s, n, i);
            if (existing != synIdx) {
                double mult = nextNodeInputMultiplier(s, n, i);
                setNextNodeInputSynapse(s, n, newCount, existing);
                setNextNodeInputMultiplier(s, n, newCount, mult);
                newCount++;
            }
        }
        setNextNodeInputCount(s, n, newCount);
    }
    int outCount = nextNodeOutputCount(s, n);
    int newOutCount = 0;
    for (int o = 0; o < outCount; ++o) {
        int existing = nextNodeOutputSynapse(s, n, o);
        if (existing != synIdx) {
            setNextNodeOutputSynapse(s, n, newOutCount, existing);
            newOutCount++;
        }
    }
    setNextNodeOutputCount(s, n, newOutCount);
}

void removeSynapseEverywhere(uint s, int synIdx) {
    int aN = nextActiveNodes(s);
    for (int n = 0; n < aN; ++n)
        removeSynapseFromNode(s, n, synIdx);
    setNextSynapseActive(s, synIdx, 0);
}

void compactNodeRemoveAt(uint s, int idx) {
    int aN = nextActiveNodes(s);
    for (int n = idx; n < aN - 1; ++n) {
        setNextNodeType(s, n, nextNodeType(s, n + 1));
        setNextNodeActivation(s, n, nextNodeActivation(s, n + 1));
        setNextNodeInputCount(s, n, nextNodeInputCount(s, n + 1));
        setNextNodeOutputCount(s, n, nextNodeOutputCount(s, n + 1));
        for (int i = 0; i < MAX_INPUTS_PER_NODE; ++i) {
            setNextNodeInputSynapse(s, n, i, nextNodeInputSynapse(s, n + 1, i));
            setNextNodeInputMultiplier(s, n, i, nextNodeInputMultiplier(s, n + 1, i));
        }
        for (int o = 0; o < MAX_OUTPUTS_PER_NODE; ++o)
            setNextNodeOutputSynapse(s, n, o, nextNodeOutputSynapse(s, n + 1, o));
    }
    int last = aN - 1;
    setNextNodeType(s, last, NODE_TYPE_INACTIVE);
    setNextNodeInputCount(s, last, 0);
    setNextNodeOutputCount(s, last, 0);
    setNextActiveNodes(s, last);
}

// Shift nodes [insertIdx .. aN-1] one slot to the right to make room at insertIdx.
// activeNodes is incremented. The slot at insertIdx still holds stale data after this
// call - callers must overwrite type/activation/inputCount/outputCount on it.
void shiftNodesRightAt(uint s, int insertIdx) {
    int aN = nextActiveNodes(s);
    for (int n = aN; n > insertIdx; --n) {
        setNextNodeType(s, n, nextNodeType(s, n - 1));
        setNextNodeActivation(s, n, nextNodeActivation(s, n - 1));
        setNextNodeInputCount(s, n, nextNodeInputCount(s, n - 1));
        setNextNodeOutputCount(s, n, nextNodeOutputCount(s, n - 1));
        for (int i = 0; i < MAX_INPUTS_PER_NODE; ++i) {
            setNextNodeInputSynapse(s, n, i, nextNodeInputSynapse(s, n - 1, i));
            setNextNodeInputMultiplier(s, n, i, nextNodeInputMultiplier(s, n - 1, i));
        }
        for (int o = 0; o < MAX_OUTPUTS_PER_NODE; ++o)
            setNextNodeOutputSynapse(s, n, o, nextNodeOutputSynapse(s, n - 1, o));
    }
    setNextActiveNodes(s, aN + 1);
}
