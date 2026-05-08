// Mirror of BiasAdderNetworkMutator: if active nodes < MaxNodes, insert a Bias at a
// random position (matches CPU's Network.Nodes.Insert). Propagation order changes
// with insertion position, so this matters for evolved network behavior.

void mutateBiasAdder(uint s, inout uint rng) {
    int aN = nextActiveNodes(s);
    if (aN >= MUTATOR_MAX_NODES) return;
    if (aN >= MAX_NODES) return; // buffer guard
    int insertIdx = rngInt(rng, aN);
    shiftNodesRightAt(s, insertIdx);
    setNextNodeType(s, insertIdx, NODE_TYPE_BIAS);
    setNextNodeActivation(s, insertIdx, ACT_LINEAR);
    setNextNodeInputCount(s, insertIdx, 0);
    setNextNodeOutputCount(s, insertIdx, 0);
}
