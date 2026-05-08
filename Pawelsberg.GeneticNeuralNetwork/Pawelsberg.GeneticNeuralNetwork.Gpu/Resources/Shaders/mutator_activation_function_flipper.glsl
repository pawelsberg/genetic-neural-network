// Mirror of ActivationFunctionNetworkMutator: pick a random node; if Neuron, set
// activation to a uniform random one of the 5 supported functions.

void mutateActivationFunctionFlipper(uint s, inout uint rng) {
    int aN = nextActiveNodes(s);
    if (aN <= 0) return;
    int n = rngInt(rng, aN);
    if (nextNodeType(s, n) != NODE_TYPE_NEURON) return;
    setNextNodeActivation(s, n, rngInt(rng, 5));
}
