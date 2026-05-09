// Mirror of NodeReordererNetworkMutator: pick two random node indices, swap their slots.
// If the two indices coincide it is a no-op (matches CPU).

void mutateNodeReorderer(uint s, inout uint rng) {
    int aN = nextActiveNodes(s);
    if (aN <= 0) return;
    int idx1 = rngInt(rng, aN);
    int idx2 = rngInt(rng, aN);
    if (idx1 == idx2) return;

    int t1 = nextNodeType(s, idx1), t2 = nextNodeType(s, idx2);
    int a1 = nextNodeActivation(s, idx1), a2 = nextNodeActivation(s, idx2);
    int ic1 = nextNodeInputCount(s, idx1), ic2 = nextNodeInputCount(s, idx2);
    int oc1 = nextNodeOutputCount(s, idx1), oc2 = nextNodeOutputCount(s, idx2);
    setNextNodeType(s, idx1, t2); setNextNodeType(s, idx2, t1);
    setNextNodeActivation(s, idx1, a2); setNextNodeActivation(s, idx2, a1);
    setNextNodeInputCount(s, idx1, ic2); setNextNodeInputCount(s, idx2, ic1);
    setNextNodeOutputCount(s, idx1, oc2); setNextNodeOutputCount(s, idx2, oc1);

    for (int i = 0; i < MAX_INPUTS_PER_NODE; ++i) {
        int s1 = nextNodeInputSynapse(s, idx1, i);
        int s2 = nextNodeInputSynapse(s, idx2, i);
        double m1 = nextNodeInputMultiplier(s, idx1, i);
        double m2 = nextNodeInputMultiplier(s, idx2, i);
        setNextNodeInputSynapse(s, idx1, i, s2);
        setNextNodeInputSynapse(s, idx2, i, s1);
        setNextNodeInputMultiplier(s, idx1, i, m2);
        setNextNodeInputMultiplier(s, idx2, i, m1);
    }
    for (int o = 0; o < MAX_OUTPUTS_PER_NODE; ++o) {
        int s1 = nextNodeOutputSynapse(s, idx1, o);
        int s2 = nextNodeOutputSynapse(s, idx2, o);
        setNextNodeOutputSynapse(s, idx1, o, s2);
        setNextNodeOutputSynapse(s, idx2, o, s1);
    }
}
