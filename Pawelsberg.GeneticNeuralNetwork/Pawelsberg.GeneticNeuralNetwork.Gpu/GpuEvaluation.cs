namespace Pawelsberg.GeneticNeuralNetwork.Gpu;

public sealed class GpuEvaluation
{
    public float Fitness { get; init; }
    public float[] PerTestCaseDiff { get; init; } = Array.Empty<float>();
    public float[] PerTestCaseScore { get; init; } = Array.Empty<float>();
    public bool[] PerTestCaseGood { get; init; } = Array.Empty<bool>();
}
