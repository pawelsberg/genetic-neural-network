// Slim common-header for update_best_ever.comp. Declares only the SSBOs that
// shader actually touches. Used instead of common.glsl because some drivers
// (NVIDIA) count every declared buffer toward GL_MAX_COMPUTE_SHADER_STORAGE_BLOCKS,
// not only the statically-active ones — so pulling in all 16 from common.glsl
// alongside the 3 bestEver buffers would overflow the limit.

#extension GL_ARB_gpu_shader_fp64 : require

layout(std430, binding = 0)  buffer GenomeIntsBuf      { int genomeI[]; };
layout(std430, binding = 1)  buffer GenomeMultsBuf     { double genomeM[]; };
layout(std430, binding = 8)  buffer FitnessBuf         { float fitness[]; };
layout(std430, binding = 10) buffer ParentByRankBuf    { int parentByRank[]; };
layout(std430, binding = 16) buffer BestEverIntsBuf    { int bestEverI[]; };
layout(std430, binding = 17) buffer BestEverMultsBuf   { double bestEverM[]; };
layout(std430, binding = 18) buffer BestEverScalarsBuf {
    float bestEverFitness;
    int bestEverNodeCount;
    int bestEverSynapseCount;
    int bestEverValid;
};

uniform uint generationIndex;

int specOffsetI(uint s) { return int(s) * INT_STRIDE; }
int activeNodes(uint s) { return genomeI[specOffsetI(s) + OFF_ACTIVE_NODES]; }
int synapseActive(uint s, int idx) { return genomeI[specOffsetI(s) + OFF_SYNAPSE_ACTIVE + idx]; }
