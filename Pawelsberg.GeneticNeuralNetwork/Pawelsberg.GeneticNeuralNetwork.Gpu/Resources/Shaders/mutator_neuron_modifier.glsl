// Mirror of NeuronModifierNetworkMutator: 50% continuous Randomize, 50% integer
// RandomizeInteger. Pick a random node; if it's a Neuron with inputs, mutate one
// input multiplier.

void mutateNeuronModifier(uint s, inout uint rng) {
    int aN = nextActiveNodes(s);
    if (aN <= 0) return;
    bool integerVariant = rngFloat(rng) < 0.5;
    int n = rngInt(rng, aN);
    if (nextNodeType(s, n) != NODE_TYPE_NEURON) return;
    int inCount = nextNodeInputCount(s, n);
    if (inCount <= 0) return;
    int i = rngInt(rng, inCount);
    float current = nextNodeInputMultiplier(s, n, i);
    float next;
    if (integerVariant) {
        // CPU clamps current to int range, then RandomizeInteger.
        int currentInt = int(clamp(current, -2147483648.0, 2147483647.0));
        next = float(rngRandomizeInteger(rng, currentInt));
    } else {
        next = rngRandomize(rng, current);
    }
    setNextNodeInputMultiplier(s, n, i, next);
}
