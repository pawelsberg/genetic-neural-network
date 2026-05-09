namespace Pawelsberg.GeneticNeuralNetwork.Gpu;

/// <summary>
/// Compile-time constants that don't vary per run. Per-run sizes (MaxNodes,
/// MaxSynapses, MaxInputsPerNode, etc.) live on GpuLayout and are computed at
/// GpuRunner construction from the seed networks + simulation caps.
/// </summary>
public static class GpuConstants
{
    public const int PopulationSize = 1000;
    public const int Generations = 10000;

    public const double GoodDifference = 0.001d;
    public const double QualityForOneDiff = 0.01d;
    public const double QualityForGoodResult = 10d;
    public const double SubstractedQualityForOneDiff = 1d;
    public const double QualityForOneNode = 5d;
    public const double QualityForOneSynapse = 5d;
    public const double QualityForMultiplierSumEqOne = 0.0001d;

    public const int NodeTypeInactive = -1;
    public const int NodeTypeNeuron = 0;
    public const int NodeTypeBias = 1;

    public const int ActivationLinear = 0;
    public const int ActivationThreshold = 1;
    public const int ActivationSquashing = 2;
    public const int ActivationSigmoid = 3;
    public const int ActivationTanh = 4;
}
