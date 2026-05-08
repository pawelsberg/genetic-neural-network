using OpenTK.Graphics.OpenGL4;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;

namespace Pawelsberg.GeneticNeuralNetwork.Gpu;

public sealed class GpuRunner : IDisposable
{
    private readonly GlContext _context;
    private readonly ComputeProgram _propagate;
    private readonly ComputeProgram _score;
    private readonly ComputeProgram _aggregate;
    private readonly ComputeProgram _rank;
    private readonly ComputeProgram _mutate;

    private readonly GpuBuffer _genomeI;
    private readonly GpuBuffer _genomeM;
    private readonly GpuBuffer _nextGenomeI;
    private readonly GpuBuffer _nextGenomeM;
    private readonly GpuBuffer _potentials;
    private readonly GpuBuffer _outputDiffs;
    private readonly GpuBuffer _perTestScores;
    private readonly GpuBuffer _perTestAllGood;
    private readonly GpuBuffer _fitness;
    private readonly GpuBuffer _specRank;
    private readonly GpuBuffer _parentByRank;
    private readonly GpuBuffer _testInputs;
    private readonly GpuBuffer _testOutputs;
    private readonly GpuBuffer _testInputCount;
    private readonly GpuBuffer _testOutputCount;
    private readonly GpuBuffer _rngStates;

    private readonly GpuLayout _layout;

    public string DeviceInfo => _context.GetDeviceInfo();
    public GpuLayout Layout => _layout;

    public GpuRunner(IList<Network> seeds, TestCaseList testCaseList, GpuLayout layout)
    {
        if (seeds == null || seeds.Count == 0)
            throw new ArgumentException("At least one seed network is required", nameof(seeds));
        if (testCaseList.TestCases.Count != layout.TestCaseCount)
            throw new ArgumentException($"Layout TestCaseCount={layout.TestCaseCount} but testCaseList has {testCaseList.TestCases.Count}");
        _layout = layout;

        _context = new GlContext();

        ShaderBuilder builder = new ShaderBuilder(layout);
        _propagate = new ComputeProgram(builder.BuildKernel("propagate.comp", false), "propagate");
        _score = new ComputeProgram(builder.BuildKernel("score_per_test_case.comp", false), "score");
        _aggregate = new ComputeProgram(builder.BuildKernel("aggregate_fitness.comp", false), "aggregate");
        _rank = new ComputeProgram(builder.BuildKernel("rank.comp", false), "rank");
        _mutate = new ComputeProgram(builder.BuildKernel("mutate.comp", true), "mutate");

        int popSize = layout.PopulationSize;
        int genomeIBytes = popSize * layout.IntStridePerSpecimen * sizeof(int);
        int genomeMBytes = popSize * layout.MultiplierStridePerSpecimen * sizeof(double);
        _genomeI = new GpuBuffer("genomeI", genomeIBytes);
        _genomeM = new GpuBuffer("genomeM", genomeMBytes);
        _nextGenomeI = new GpuBuffer("nextGenomeI", genomeIBytes);
        _nextGenomeM = new GpuBuffer("nextGenomeM", genomeMBytes);

        _potentials = new GpuBuffer("potentials", popSize * layout.TestCaseCount * layout.MaxSynapses * sizeof(double));
        _outputDiffs = new GpuBuffer("outputDiffs", popSize * layout.TestCaseCount * sizeof(float));
        _perTestScores = new GpuBuffer("perTestScores", popSize * layout.TestCaseCount * sizeof(float));
        _perTestAllGood = new GpuBuffer("perTestAllGood", popSize * layout.TestCaseCount * sizeof(int));

        _fitness = new GpuBuffer("fitness", popSize * sizeof(float));
        _specRank = new GpuBuffer("specRank", popSize * sizeof(int));
        _parentByRank = new GpuBuffer("parentByRank", popSize * sizeof(int));

        _testInputs = new GpuBuffer("testInputs", layout.TestCaseCount * layout.MaxTestCaseInputs * sizeof(float));
        _testOutputs = new GpuBuffer("testOutputs", layout.TestCaseCount * layout.MaxTestCaseOutputs * sizeof(float));
        _testInputCount = new GpuBuffer("testInputCount", layout.TestCaseCount * sizeof(int));
        _testOutputCount = new GpuBuffer("testOutputCount", layout.TestCaseCount * sizeof(int));
        _rngStates = new GpuBuffer("rngStates", popSize * sizeof(uint));

        UploadInitialPopulation(seeds);
        UploadTestCases(testCaseList);
        UploadRngStates();
    }

    private void UploadInitialPopulation(IList<Network> seeds)
    {
        FlatGenome[] encoded = new FlatGenome[seeds.Count];
        for (int s = 0; s < seeds.Count; s++)
            encoded[s] = FlatGenome.Encode(seeds[s], _layout);

        int popSize = _layout.PopulationSize;
        int intStride = _layout.IntStridePerSpecimen;
        int multStride = _layout.MultiplierStridePerSpecimen;

        int[] allInts = new int[popSize * intStride];
        double[] allMults = new double[popSize * multStride];
        for (int s = 0; s < popSize; s++)
        {
            FlatGenome g = encoded[s % encoded.Length];
            Array.Copy(g.Ints, 0, allInts, s * intStride, intStride);
            Array.Copy(g.Multipliers, 0, allMults, s * multStride, multStride);
        }
        _genomeI.UploadInts(allInts);
        _genomeM.UploadDoubles(allMults);
    }

    private void UploadTestCases(TestCaseList testCaseList)
    {
        int tcCount = _layout.TestCaseCount;
        int maxIn = _layout.MaxTestCaseInputs;
        int maxOut = _layout.MaxTestCaseOutputs;
        float[] inputs = new float[tcCount * maxIn];
        float[] outputs = new float[tcCount * maxOut];
        int[] inputCounts = new int[tcCount];
        int[] outputCounts = new int[tcCount];
        for (int t = 0; t < tcCount; t++)
        {
            TestCase tc = testCaseList.TestCases[t];
            inputCounts[t] = tc.Inputs.Count;
            outputCounts[t] = tc.Outputs.Count;
            for (int i = 0; i < tc.Inputs.Count; i++)
                inputs[t * maxIn + i] = (float)tc.Inputs[i];
            for (int i = 0; i < tc.Outputs.Count; i++)
                outputs[t * maxOut + i] = (float)tc.Outputs[i];
        }
        _testInputs.UploadFloats(inputs);
        _testOutputs.UploadFloats(outputs);
        _testInputCount.UploadInts(inputCounts);
        _testOutputCount.UploadInts(outputCounts);
    }

    private void UploadRngStates()
    {
        int popSize = _layout.PopulationSize;
        int[] seeds = new int[popSize];
        for (int s = 0; s < popSize; s++)
        {
            uint v = (uint)(s + 1) * 0x9E3779B9u;
            v ^= 0xDEADBEEFu;
            if (v == 0u) v = 0x12345678u;
            seeds[s] = unchecked((int)v);
        }
        _rngStates.UploadInts(seeds);
    }

    public Network Run(int generations, int progressInterval = 0, Action<int, float>? onProgress = null)
    {
        bool genomeIsCurrent = true;
        int popSize = _layout.PopulationSize;
        int popGroups = (popSize + 63) / 64;
        int specGroups = (popSize + 15) / 16;
        int tcGroups = (_layout.TestCaseCount + 3) / 4;

        for (int gen = 0; gen < generations; ++gen)
        {
            BindBuffers(genomeIsCurrent);
            DispatchEvaluate(specGroups, tcGroups, popGroups, gen);

            if (onProgress != null && progressInterval > 0 &&
                (gen == 0 || (gen + 1) % progressInterval == 0))
            {
                int bestSpec = _parentByRank.DownloadInts(1)[0];
                float[] fits = _fitness.DownloadFloats(_layout.PopulationSize);
                onProgress(gen + 1, fits[bestSpec]);
            }

            _mutate.Use();
            GL.Uniform1(_mutate.GetUniformLocation("generationIndex"), (uint)gen);
            _mutate.Dispatch(popGroups);
            GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);

            genomeIsCurrent = !genomeIsCurrent;
        }

        BindBuffers(genomeIsCurrent);
        DispatchEvaluate(specGroups, tcGroups, popGroups, generations);

        int bestSpecimen = _parentByRank.DownloadInts(1)[0];

        GpuBuffer currentI = genomeIsCurrent ? _genomeI : _nextGenomeI;
        GpuBuffer currentM = genomeIsCurrent ? _genomeM : _nextGenomeM;
        int[] allInts = currentI.DownloadInts(_layout.PopulationSize * _layout.IntStridePerSpecimen);
        double[] allMults = currentM.DownloadDoubles(_layout.PopulationSize * _layout.MultiplierStridePerSpecimen);

        int[] specInts = new int[_layout.IntStridePerSpecimen];
        double[] specMults = new double[_layout.MultiplierStridePerSpecimen];
        Array.Copy(allInts, bestSpecimen * _layout.IntStridePerSpecimen, specInts, 0, _layout.IntStridePerSpecimen);
        Array.Copy(allMults, bestSpecimen * _layout.MultiplierStridePerSpecimen, specMults, 0, _layout.MultiplierStridePerSpecimen);

        return FlatGenome.Decode(specInts, specMults, _layout);
    }

    public float ReadBestFitness()
    {
        float[] fits = _fitness.DownloadFloats(_layout.PopulationSize);
        int[] pbr = _parentByRank.DownloadInts(1);
        return fits[pbr[0]];
    }

    /// <summary>
    /// Evaluate a single network on the GPU pipeline (no evolution). Returns the GPU
    /// fitness plus per-test-case diff/score/allGood. Replaces the entire population
    /// buffer, so call only outside the evolution loop.
    /// </summary>
    public GpuEvaluation Evaluate(Network network)
    {
        FlatGenome g = FlatGenome.Encode(network, _layout);
        int popSize = _layout.PopulationSize;
        int intStride = _layout.IntStridePerSpecimen;
        int multStride = _layout.MultiplierStridePerSpecimen;

        int[] allInts = new int[popSize * intStride];
        double[] allMults = new double[popSize * multStride];
        Array.Copy(g.Ints, 0, allInts, 0, intStride);
        Array.Copy(g.Multipliers, 0, allMults, 0, multStride);
        _genomeI.UploadInts(allInts);
        _genomeM.UploadDoubles(allMults);

        int popGroups = (popSize + 63) / 64;
        int specGroups = (popSize + 15) / 16;
        int tcGroups = (_layout.TestCaseCount + 3) / 4;

        BindBuffers(true);
        DispatchEvaluate(specGroups, tcGroups, popGroups, 0);

        float[] diffsAll = _outputDiffs.DownloadFloats(_layout.TestCaseCount);
        float[] scoresAll = _perTestScores.DownloadFloats(_layout.TestCaseCount);
        int[] allGoodAll = _perTestAllGood.DownloadInts(_layout.TestCaseCount);
        float[] fitsAll = _fitness.DownloadFloats(1);

        bool[] good = new bool[_layout.TestCaseCount];
        for (int t = 0; t < _layout.TestCaseCount; t++) good[t] = allGoodAll[t] != 0;

        return new GpuEvaluation
        {
            Fitness = fitsAll[0],
            PerTestCaseDiff = diffsAll,
            PerTestCaseScore = scoresAll,
            PerTestCaseGood = good
        };
    }

    private void DispatchEvaluate(int specGroups, int tcGroups, int popGroups, int gen)
    {
        _propagate.Use();
        GL.Uniform1(_propagate.GetUniformLocation("generationIndex"), (uint)gen);
        _propagate.Dispatch(specGroups, tcGroups);
        GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);

        _score.Use();
        GL.Uniform1(_score.GetUniformLocation("generationIndex"), (uint)gen);
        _score.Dispatch(specGroups, tcGroups);
        GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);

        _aggregate.Use();
        GL.Uniform1(_aggregate.GetUniformLocation("generationIndex"), (uint)gen);
        _aggregate.Dispatch(popGroups);
        GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);

        _rank.Use();
        GL.Uniform1(_rank.GetUniformLocation("generationIndex"), (uint)gen);
        _rank.Dispatch(popGroups);
        GL.MemoryBarrier(MemoryBarrierFlags.ShaderStorageBarrierBit);
    }

    private void BindBuffers(bool genomeIsCurrent)
    {
        if (genomeIsCurrent)
        {
            _genomeI.Bind(0); _genomeM.Bind(1);
            _nextGenomeI.Bind(2); _nextGenomeM.Bind(3);
        }
        else
        {
            _nextGenomeI.Bind(0); _nextGenomeM.Bind(1);
            _genomeI.Bind(2); _genomeM.Bind(3);
        }
        _potentials.Bind(4);
        _outputDiffs.Bind(5);
        _perTestScores.Bind(6);
        _perTestAllGood.Bind(7);
        _fitness.Bind(8);
        _specRank.Bind(9);
        _parentByRank.Bind(10);
        _testInputs.Bind(11);
        _testOutputs.Bind(12);
        _rngStates.Bind(13);
        _testInputCount.Bind(14);
        _testOutputCount.Bind(15);
    }

    public void Dispose()
    {
        _propagate.Dispose();
        _score.Dispose();
        _aggregate.Dispose();
        _rank.Dispose();
        _mutate.Dispose();
        _genomeI.Dispose();
        _genomeM.Dispose();
        _nextGenomeI.Dispose();
        _nextGenomeM.Dispose();
        _potentials.Dispose();
        _outputDiffs.Dispose();
        _perTestScores.Dispose();
        _perTestAllGood.Dispose();
        _fitness.Dispose();
        _specRank.Dispose();
        _parentByRank.Dispose();
        _testInputs.Dispose();
        _testOutputs.Dispose();
        _testInputCount.Dispose();
        _testOutputCount.Dispose();
        _rngStates.Dispose();
        _context.Dispose();
    }
}
