using System.Globalization;
using System.Reflection;
using System.Text;

namespace Pawelsberg.GeneticNeuralNetwork.Gpu;

internal sealed class ShaderBuilder
{
    public GpuLayout Layout { get; }

    // CPU CreateNormal weights, scaled x10 so they remain integers (CPU uses 30, 15, ..., 0.1).
    public const int WeightNeuronModifier = 300;
    public const int WeightSynapseAdder = 150;
    public const int WeightSynapseRemover = 50;
    public const int WeightSynapseReconnector = 150;
    public const int WeightActivationFlipper = 50;
    public const int WeightBiasAdder = 30;
    public const int WeightNeuronAdder = 30;
    public const int WeightNodeRemover = 50;
    public const int WeightNodeReorderer = 20;
    public const int WeightNeuronMerger = 50;
    public const int WeightInputRemover = 10;
    public const int WeightOutputRemover = 10;
    public const int WeightSynapseReducer = 10;
    public const int WeightNeuronReducer = 10;
    public const int WeightTransparentNeuronAdder = 10;
    public const int WeightNothingDoer = 10;
    public const int WeightInputAdder = 1;
    public const int WeightOutputAdder = 1;
    public const int WeightMultipleTimes = 200;

    public ShaderBuilder(GpuLayout layout)
    {
        Layout = layout;
    }

    public string BuildKernel(string mainCompResourceName, bool includeMutators)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("#version 430 core");
        AppendDefines(sb);
        // update_best_ever uses a slim header (common_best_ever.glsl) declaring only
        // the SSBOs it touches. Including the full common.glsl would push that shader
        // past GL_MAX_COMPUTE_SHADER_STORAGE_BLOCKS on drivers that count declared
        // (not just statically-active) buffers.
        string commonResource = mainCompResourceName == "update_best_ever.comp"
            ? "common_best_ever.glsl"
            : "common.glsl";
        sb.AppendLine(LoadResource(commonResource));
        if (includeMutators)
        {
            sb.AppendLine(LoadResource("genome_rw.glsl"));
            sb.AppendLine(LoadResource("mutator_neuron_modifier.glsl"));
            sb.AppendLine(LoadResource("mutator_synapse_adder.glsl"));
            sb.AppendLine(LoadResource("mutator_synapse_remover.glsl"));
            sb.AppendLine(LoadResource("mutator_synapse_reconnector.glsl"));
            sb.AppendLine(LoadResource("mutator_activation_function_flipper.glsl"));
            sb.AppendLine(LoadResource("mutator_bias_adder.glsl"));
            sb.AppendLine(LoadResource("mutator_neuron_adder.glsl"));
            sb.AppendLine(LoadResource("mutator_node_remover.glsl"));
            sb.AppendLine(LoadResource("mutator_node_reorderer.glsl"));
            sb.AppendLine(LoadResource("mutator_neuron_merger.glsl"));
            sb.AppendLine(LoadResource("mutator_input_adder.glsl"));
            sb.AppendLine(LoadResource("mutator_input_remover.glsl"));
            sb.AppendLine(LoadResource("mutator_output_adder.glsl"));
            sb.AppendLine(LoadResource("mutator_output_remover.glsl"));
            sb.AppendLine(LoadResource("mutator_synapse_reducer.glsl"));
            sb.AppendLine(LoadResource("mutator_neuron_reducer.glsl"));
            sb.AppendLine(LoadResource("mutator_transparent_neuron_adder.glsl"));
            sb.AppendLine(LoadResource("mutator_nothing_doer.glsl"));
        }
        sb.AppendLine(LoadResource(mainCompResourceName));
        return sb.ToString();
    }

    private void AppendDefines(StringBuilder sb)
    {
        sb.AppendLine($"#define POPULATION_SIZE {Layout.PopulationSize}");
        sb.AppendLine($"#define MAX_NODES {Layout.MaxNodes}");
        sb.AppendLine($"#define MAX_SYNAPSES {Layout.MaxSynapses}");
        sb.AppendLine($"#define MAX_INPUTS_PER_NODE {Layout.MaxInputsPerNode}");
        sb.AppendLine($"#define MAX_OUTPUTS_PER_NODE {Layout.MaxOutputsPerNode}");
        sb.AppendLine($"#define MAX_NETWORK_INPUTS {Layout.MaxNetworkInputs}");
        sb.AppendLine($"#define MAX_NETWORK_OUTPUTS {Layout.MaxNetworkOutputs}");
        sb.AppendLine($"#define MAX_TEST_CASE_INPUTS {Layout.MaxTestCaseInputs}");
        sb.AppendLine($"#define MAX_TEST_CASE_OUTPUTS {Layout.MaxTestCaseOutputs}");
        sb.AppendLine($"#define MUTATOR_MAX_NODES {Layout.MutatorMaxNodes}");
        sb.AppendLine($"#define MUTATOR_MAX_SYNAPSES {Layout.MutatorMaxSynapses}");
        sb.AppendLine($"#define INT_STRIDE {Layout.IntStridePerSpecimen}");
        sb.AppendLine($"#define MULT_STRIDE {Layout.MultiplierStridePerSpecimen}");

        sb.AppendLine($"#define OFF_ACTIVE_NODES {Layout.OffActiveNodes}");
        sb.AppendLine($"#define OFF_NETWORK_INPUT_COUNT {Layout.OffNetworkInputCount}");
        sb.AppendLine($"#define OFF_NETWORK_OUTPUT_COUNT {Layout.OffNetworkOutputCount}");
        sb.AppendLine($"#define OFF_NODE_TYPE {Layout.OffNodeType}");
        sb.AppendLine($"#define OFF_NODE_ACTIVATION {Layout.OffNodeActivation}");
        sb.AppendLine($"#define OFF_NODE_INPUT_COUNT {Layout.OffNodeInputCount}");
        sb.AppendLine($"#define OFF_NODE_INPUT_SYNAPSE {Layout.OffNodeInputSynapse}");
        sb.AppendLine($"#define OFF_NODE_OUTPUT_COUNT {Layout.OffNodeOutputCount}");
        sb.AppendLine($"#define OFF_NODE_OUTPUT_SYNAPSE {Layout.OffNodeOutputSynapse}");
        sb.AppendLine($"#define OFF_NETWORK_INPUT_SYNAPSE {Layout.OffNetworkInputSynapse}");
        sb.AppendLine($"#define OFF_NETWORK_OUTPUT_SYNAPSE {Layout.OffNetworkOutputSynapse}");
        sb.AppendLine($"#define OFF_SYNAPSE_ACTIVE {Layout.OffSynapseActive}");

        sb.AppendLine($"#define NODE_TYPE_INACTIVE ({GpuConstants.NodeTypeInactive})");
        sb.AppendLine($"#define NODE_TYPE_NEURON {GpuConstants.NodeTypeNeuron}");
        sb.AppendLine($"#define NODE_TYPE_BIAS {GpuConstants.NodeTypeBias}");

        sb.AppendLine($"#define ACT_LINEAR {GpuConstants.ActivationLinear}");
        sb.AppendLine($"#define ACT_THRESHOLD {GpuConstants.ActivationThreshold}");
        sb.AppendLine($"#define ACT_SQUASHING {GpuConstants.ActivationSquashing}");
        sb.AppendLine($"#define ACT_SIGMOID {GpuConstants.ActivationSigmoid}");
        sb.AppendLine($"#define ACT_TANH {GpuConstants.ActivationTanh}");

        sb.AppendLine($"#define PROPAGATIONS {Layout.Propagations}");
        sb.AppendLine($"#define TEST_CASE_COUNT {Layout.TestCaseCount}");

        sb.AppendLine($"#define GOOD_DIFFERENCE {GpuConstants.GoodDifference.ToString("R", CultureInfo.InvariantCulture)}");
        sb.AppendLine($"#define QUALITY_FOR_ONE_DIFF {GpuConstants.QualityForOneDiff.ToString("R", CultureInfo.InvariantCulture)}");
        sb.AppendLine($"#define QUALITY_FOR_GOOD_RESULT {GpuConstants.QualityForGoodResult.ToString("R", CultureInfo.InvariantCulture)}");
        sb.AppendLine($"#define SUBSTRACTED_QUALITY_FOR_ONE_DIFF {GpuConstants.SubstractedQualityForOneDiff.ToString("R", CultureInfo.InvariantCulture)}");
        sb.AppendLine($"#define QUALITY_FOR_ONE_NODE {GpuConstants.QualityForOneNode.ToString("R", CultureInfo.InvariantCulture)}");
        sb.AppendLine($"#define QUALITY_FOR_ONE_SYNAPSE {GpuConstants.QualityForOneSynapse.ToString("R", CultureInfo.InvariantCulture)}");
        sb.AppendLine($"#define QUALITY_FOR_MULTIPLIER_SUM_EQ_ONE {GpuConstants.QualityForMultiplierSumEqOne.ToString("R", CultureInfo.InvariantCulture)}");
        sb.AppendLine($"#define QUALITY_FOR_ONE_MS 25.0");

        int baseTotal =
            WeightNeuronModifier + WeightSynapseAdder + WeightSynapseRemover +
            WeightSynapseReconnector + WeightActivationFlipper + WeightBiasAdder +
            WeightNeuronAdder + WeightNodeRemover + WeightNodeReorderer +
            WeightNeuronMerger + WeightInputRemover + WeightOutputRemover +
            WeightSynapseReducer + WeightNeuronReducer + WeightTransparentNeuronAdder +
            WeightNothingDoer + WeightInputAdder + WeightOutputAdder;
        int total = baseTotal + WeightMultipleTimes;
        sb.AppendLine($"#define WEIGHT_NEURON_MODIFIER {WeightNeuronModifier}");
        sb.AppendLine($"#define WEIGHT_SYNAPSE_ADDER {WeightSynapseAdder}");
        sb.AppendLine($"#define WEIGHT_SYNAPSE_REMOVER {WeightSynapseRemover}");
        sb.AppendLine($"#define WEIGHT_SYNAPSE_RECONNECTOR {WeightSynapseReconnector}");
        sb.AppendLine($"#define WEIGHT_ACTIVATION_FLIPPER {WeightActivationFlipper}");
        sb.AppendLine($"#define WEIGHT_BIAS_ADDER {WeightBiasAdder}");
        sb.AppendLine($"#define WEIGHT_NEURON_ADDER {WeightNeuronAdder}");
        sb.AppendLine($"#define WEIGHT_NODE_REMOVER {WeightNodeRemover}");
        sb.AppendLine($"#define WEIGHT_NODE_REORDERER {WeightNodeReorderer}");
        sb.AppendLine($"#define WEIGHT_NEURON_MERGER {WeightNeuronMerger}");
        sb.AppendLine($"#define WEIGHT_INPUT_REMOVER {WeightInputRemover}");
        sb.AppendLine($"#define WEIGHT_OUTPUT_REMOVER {WeightOutputRemover}");
        sb.AppendLine($"#define WEIGHT_SYNAPSE_REDUCER {WeightSynapseReducer}");
        sb.AppendLine($"#define WEIGHT_NEURON_REDUCER {WeightNeuronReducer}");
        sb.AppendLine($"#define WEIGHT_TRANSPARENT_NEURON_ADDER {WeightTransparentNeuronAdder}");
        sb.AppendLine($"#define WEIGHT_NOTHING_DOER {WeightNothingDoer}");
        sb.AppendLine($"#define WEIGHT_INPUT_ADDER {WeightInputAdder}");
        sb.AppendLine($"#define WEIGHT_OUTPUT_ADDER {WeightOutputAdder}");
        sb.AppendLine($"#define WEIGHT_MULTIPLE_TIMES {WeightMultipleTimes}");
        sb.AppendLine($"#define BASE_MUTATOR_WEIGHT {baseTotal}");
        sb.AppendLine($"#define TOTAL_MUTATOR_WEIGHT {total}");
    }

    private static string LoadResource(string fileName)
    {
        Assembly asm = typeof(ShaderBuilder).Assembly;
        string suffix = "." + fileName;
        string? fullName = null;
        foreach (string n in asm.GetManifestResourceNames())
            if (n.EndsWith(suffix, StringComparison.Ordinal)) { fullName = n; break; }
        if (fullName == null)
            throw new Exception($"Embedded shader resource '{fileName}' not found");
        using Stream stream = asm.GetManifestResourceStream(fullName)!;
        using StreamReader reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
