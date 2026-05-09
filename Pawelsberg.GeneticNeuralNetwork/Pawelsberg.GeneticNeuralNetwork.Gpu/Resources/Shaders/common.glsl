// SSBO declarations and shared helpers. Constants come from C#-prepended #defines.

#extension GL_ARB_gpu_shader_fp64 : require

layout(std430, binding = 0) buffer GenomeIntsBuf { int genomeI[]; };
layout(std430, binding = 1) buffer GenomeMultsBuf { double genomeM[]; };
layout(std430, binding = 2) buffer NextGenomeIntsBuf { int nextGenomeI[]; };
layout(std430, binding = 3) buffer NextGenomeMultsBuf { double nextGenomeM[]; };
layout(std430, binding = 4) buffer SynapsePotentialsBuf { double synapsePotentials[]; };
layout(std430, binding = 5) buffer OutputDiffBuf { float outputDiffs[]; };
layout(std430, binding = 6) buffer PerTestScoreBuf { float perTestScores[]; };
layout(std430, binding = 7) buffer PerTestAllGoodBuf { int perTestAllGood[]; };
layout(std430, binding = 8) buffer FitnessBuf { float fitness[]; };
layout(std430, binding = 9) buffer RankBuf { int specRank[]; };
layout(std430, binding = 10) buffer ParentByRankBuf { int parentByRank[]; };
layout(std430, binding = 11) buffer TestCaseInputsBuf { float testCaseInputs[]; };
layout(std430, binding = 12) buffer TestCaseOutputsBuf { float testCaseOutputs[]; };
layout(std430, binding = 13) buffer RngStateBuf { uint rngStates[]; };
layout(std430, binding = 14) buffer TestCaseInputCountBuf { int testCaseInputCount[]; };
layout(std430, binding = 15) buffer TestCaseOutputCountBuf { int testCaseOutputCount[]; };
// Bindings 16-18 (bestEverI, bestEverM, bestEverScalars) are declared only inside
// update_best_ever.comp — keeping them out of common.glsl keeps the per-shader
// SSBO declaration count under GL_MAX_COMPUTE_SHADER_STORAGE_BLOCKS on drivers
// (NVIDIA) that count declared blocks, not only statically-used ones.

uniform uint generationIndex;

int specOffsetI(uint s) { return int(s) * INT_STRIDE; }
int specOffsetM(uint s) { return int(s) * MULT_STRIDE; }

int activeNodes(uint s) { return genomeI[specOffsetI(s) + OFF_ACTIVE_NODES]; }
int networkInputCount(uint s) { return genomeI[specOffsetI(s) + OFF_NETWORK_INPUT_COUNT]; }
int networkOutputCount(uint s) { return genomeI[specOffsetI(s) + OFF_NETWORK_OUTPUT_COUNT]; }
int nodeType(uint s, int n) { return genomeI[specOffsetI(s) + OFF_NODE_TYPE + n]; }
int nodeActivation(uint s, int n) { return genomeI[specOffsetI(s) + OFF_NODE_ACTIVATION + n]; }
int nodeInputCount(uint s, int n) { return genomeI[specOffsetI(s) + OFF_NODE_INPUT_COUNT + n]; }
int nodeOutputCount(uint s, int n) { return genomeI[specOffsetI(s) + OFF_NODE_OUTPUT_COUNT + n]; }
int nodeInputSynapse(uint s, int n, int i) { return genomeI[specOffsetI(s) + OFF_NODE_INPUT_SYNAPSE + n * MAX_INPUTS_PER_NODE + i]; }
int nodeOutputSynapse(uint s, int n, int o) { return genomeI[specOffsetI(s) + OFF_NODE_OUTPUT_SYNAPSE + n * MAX_OUTPUTS_PER_NODE + o]; }
double nodeInputMultiplier(uint s, int n, int i) { return genomeM[specOffsetM(s) + n * MAX_INPUTS_PER_NODE + i]; }
int networkInputSynapse(uint s, int i) { return genomeI[specOffsetI(s) + OFF_NETWORK_INPUT_SYNAPSE + i]; }
int networkOutputSynapse(uint s, int i) { return genomeI[specOffsetI(s) + OFF_NETWORK_OUTPUT_SYNAPSE + i]; }
int synapseActive(uint s, int idx) { return genomeI[specOffsetI(s) + OFF_SYNAPSE_ACTIVE + idx]; }

int potOffset(uint spec, uint tc) { return int(spec) * TEST_CASE_COUNT * MAX_SYNAPSES + int(tc) * MAX_SYNAPSES; }

float applyActivation(int act, float x) {
    if (act == ACT_LINEAR) return x;
    if (act == ACT_THRESHOLD) return clamp(x, -1.0, 1.0);
    if (act == ACT_SQUASHING) return clamp(x, 0.0, 1.0);
    if (act == ACT_SIGMOID) return 1.0 / (1.0 + exp(-x));
    if (act == ACT_TANH) return tanh(x);
    return x;
}

// Double-precision exp via range reduction x = k*ln(2) + r and Taylor on r.
// |r| <= ln(2)/2 makes 12 Taylor terms accurate to ~16 digits.
// Then multiply by 2^k via O(log|k|) doubling.
double expD(double x) {
    if (isnan(x)) return x;
    if (x > 700.0lf) return 1.0e300lf;
    if (x < -700.0lf) return 0.0lf;

    const double LN2 = 0.6931471805599453094172321lf;
    const double LN2_INV = 1.4426950408889634073599247lf;

    int k = int(floor(x * LN2_INV + 0.5lf));
    double r = x - double(k) * LN2;

    double term = 1.0lf;
    double sum = 1.0lf;
    term *= r / 1.0lf;  sum += term;
    term *= r / 2.0lf;  sum += term;
    term *= r / 3.0lf;  sum += term;
    term *= r / 4.0lf;  sum += term;
    term *= r / 5.0lf;  sum += term;
    term *= r / 6.0lf;  sum += term;
    term *= r / 7.0lf;  sum += term;
    term *= r / 8.0lf;  sum += term;
    term *= r / 9.0lf;  sum += term;
    term *= r / 10.0lf; sum += term;
    term *= r / 11.0lf; sum += term;
    term *= r / 12.0lf; sum += term;

    int absK = k < 0 ? -k : k;
    double base2 = 2.0lf;
    double pow2 = 1.0lf;
    for (int bit = 0; bit < 11; ++bit) {
        if ((absK & (1 << bit)) != 0) pow2 *= base2;
        base2 *= base2;
    }
    if (k < 0) pow2 = 1.0lf / pow2;
    return sum * pow2;
}

double tanhD(double x) {
    if (x > 20.0lf) return 1.0lf;
    if (x < -20.0lf) return -1.0lf;
    double e2x = expD(2.0lf * x);
    return (e2x - 1.0lf) / (e2x + 1.0lf);
}

double applyActivationD(int act, double x) {
    if (act == ACT_LINEAR) return x;
    if (act == ACT_THRESHOLD) return clamp(x, -1.0lf, 1.0lf);
    if (act == ACT_SQUASHING) return clamp(x, 0.0lf, 1.0lf);
    if (act == ACT_SIGMOID) return 1.0lf / (1.0lf + expD(-x));
    if (act == ACT_TANH) return tanhD(x);
    return x;
}

uint rngNext(inout uint state) {
    uint x = state;
    if (x == 0u) x = 0x9E3779B9u;
    x ^= x << 13u;
    x ^= x >> 17u;
    x ^= x << 5u;
    state = x;
    return x;
}

float rngFloat(inout uint state) {
    return float(rngNext(state) & 0x00FFFFFFu) / float(0x01000000u);
}

int rngInt(inout uint state, int upperExclusive) {
    if (upperExclusive <= 0) return 0;
    return int(rngNext(state) % uint(upperExclusive));
}

float rngRandomValue(inout uint state) {
    float coin1 = rngFloat(state);
    if (coin1 < 0.5) return (rngFloat(state) - 0.5) * 4.0;
    float coin2 = rngFloat(state);
    if (coin2 < 0.5) return (rngFloat(state) - 0.5) * 0.001;
    return (rngFloat(state) - 0.5) * 20.0;
}

int rngRandomInteger(inout uint state) {
    if (rngFloat(state) < 0.5) return rngInt(state, 2) - 1;
    if (rngFloat(state) < 0.5) return rngInt(state, 6) - 3;
    if (rngFloat(state) < 0.5) return rngInt(state, 20) - 10;
    return rngInt(state, 100) - 50;
}

float rngRandomize(inout uint state, float value) {
    if (rngFloat(state) < 0.5) return value + rngRandomValue(state);
    float result = rngRandomValue(state);
    if (rngFloat(state) < 0.5) result += 1.0;
    return result;
}

int rngRandomizeInteger(inout uint state, int value) {
    if (rngFloat(state) < 0.5) return rngRandomInteger(state);
    return value + rngRandomInteger(state);
}
