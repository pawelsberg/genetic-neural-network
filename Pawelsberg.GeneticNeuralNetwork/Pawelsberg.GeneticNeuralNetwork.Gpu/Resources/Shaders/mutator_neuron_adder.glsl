// Mirror of NeuronAdderNetworkMutator: if active nodes < MaxNodes, insert a Neuron
// (default Linear activation) at a random position (matches CPU's Network.Nodes.Insert).

void mutateNeuronAdder(uint s, inout uint rng) {
    int aN = nextActiveNodes(s);
    if (aN >= MUTATOR_MAX_NODES) return;
    if (aN >= MAX_NODES) return; // buffer guard
    int insertIdx = rngInt(rng, aN);
    shiftNodesRightAt(s, insertIdx);
    setNextNodeType(s, insertIdx, NODE_TYPE_NEURON);
    setNextNodeActivation(s, insertIdx, ACT_LINEAR);
    setNextNodeInputCount(s, insertIdx, 0);
    setNextNodeOutputCount(s, insertIdx, 0);
}
