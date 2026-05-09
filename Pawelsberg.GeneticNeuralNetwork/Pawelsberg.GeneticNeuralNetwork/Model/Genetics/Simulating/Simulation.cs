using Pawelsberg.GeneticNeuralNetwork.Model.Genetics.QueuingParents;

namespace Pawelsberg.GeneticNeuralNetwork.Model.Genetics.Simulating;

public class Simulation<TSpecimen> where TSpecimen : ISpecimen
{
    public SimulationTimer SimulationTimer { get; private set; }
    public Log Log { get; private set; }

    public TSpecimen BestEver { get; private set; }
    public double MaxPossibleQuality { get; private set; }
    public double BestEverQuality { get; private set; }
    public int GenerationNumber { get; private set; }

    private readonly object _parametersLock;
    private Generation<TSpecimen> _generation;

    private readonly List<TSpecimen> _specsToAdd;
    private bool _clearSpecs;
    private bool _reevaluateBest;

    private int _maxSpecimens;
    public int MaxSpecimens
    {
        get { return _maxSpecimens; }
        set
        {
            lock (_parametersLock)
                _maxSpecimens = value;
        }
    }

    private int _generationMultiplier;
    public int GenerationMultiplier
    {
        get { return _generationMultiplier; }
        set
        {
            lock (_parametersLock)
                _generationMultiplier = value;
        }
    }

    private QualityMeter<TSpecimen> _generationMeter;
    public QualityMeter<TSpecimen> GenerationMeter
    {
        get { return _generationMeter; }
        set
        {
            lock (_parametersLock)
            {
                _generationMeter = value;
                MaxPossibleQuality = _generationMeter.MaxQualityRecursive ?? 0d;
                _reevaluateBest = true;
            }
        }
    }

    /// <summary>
    /// Recalculates MaxPossibleQuality from the current GenerationMeter.
    /// Call this when the quality meter's children change dynamically (e.g., after updating TestCaseList).
    /// Also triggers reevaluation of BestEver quality on next generation tick.
    /// </summary>
    public void RecalculateMaxPossibleQuality()
    {
        lock (_parametersLock)
        {
            if (_generationMeter != null)
            {
                MaxPossibleQuality = _generationMeter.MaxQualityRecursive ?? 0d;
                _reevaluateBest = true;
            }
        }
    }

    private QualityMeter<TSpecimen> _simulationMeter;
    public QualityMeter<TSpecimen> SimulationMeter
    {
        get { return _simulationMeter; }
        set
        {
            lock (_parametersLock)
            {
                _simulationMeter = value;
                _reevaluateBest = true;
            }
        }
    }

    private ParentQueuer<TSpecimen> _parentQueuer;
    public ParentQueuer<TSpecimen> ParentQueuer
    {
        get { return _parentQueuer; }
        set
        {
            lock (_parametersLock)
            {
                _parentQueuer = value;
            }
        }
    }

    private Mutators<TSpecimen> _mutators;
    public Mutators<TSpecimen> Mutators
    {
        get { return _mutators; }
        set
        {
            lock (_parametersLock)
            {
                _mutators = value;
            }
        }
    }

    public event EventHandler<NextGenerationCreatedEventArgs<TSpecimen>> NextGenerationCreated;

    public Simulation()
    {
        SimulationTimer = new SimulationTimer(5);
        SimulationTimer.Ticked += new EventHandler(SimulationTimer_Ticked);
        Log = new Log(5);

        _parametersLock = new object();
        MaxSpecimens = 4;
        GenerationMultiplier = 5;
        _specsToAdd = new List<TSpecimen>();
        _generation = new Generation<TSpecimen>();

        GenerationNumber = 0;
        BestEverQuality = -1d;
        _reevaluateBest = true;
    }

    /// <summary>  </summary>
    /// <param name="nextGeneration"></param>
    /// <param name="prevGeneration"></param>
    /// <returns> returns true if to pause </returns>
    private bool OnNextGenerationCreated(Generation<TSpecimen> nextGeneration, Generation<TSpecimen> prevGeneration)
    {
        if (NextGenerationCreated != null)
        {
            NextGenerationCreatedEventArgs<TSpecimen> nextGenerationCreatedEventArgs = new NextGenerationCreatedEventArgs<TSpecimen>(nextGeneration, prevGeneration);
            NextGenerationCreated(this, nextGenerationCreatedEventArgs);
            return nextGenerationCreatedEventArgs.Pause;
        }
        else
            return false;
    }

    public void Add(TSpecimen specimen)
    {
        lock (_parametersLock)
        {
            _specsToAdd.Add(specimen);
        }
    }

    /// <summary>
    /// Like Add() but additionally re-evaluates the candidate against current GenerationMeter
    /// and updates BestEver synchronously if it's better (with SimulationMeter tiebreak on
    /// equal fitness, matching the per-tick logic). Use when an external evaluator (the GPU
    /// runner) hands a specimen to the CPU sim and the user expects `show` to reflect it
    /// immediately, without first running a CPU tick. Quality measurement happens outside
    /// the lock so a slow meter doesn't stall a concurrent CPU tick; the BestEver update
    /// re-checks under the lock to avoid clobbering a concurrent improvement.
    /// </summary>
    public void AddAndRefreshBestEver(TSpecimen specimen)
    {
        QualityMeter<TSpecimen> meter;
        QualityMeter<TSpecimen> simMeter;
        lock (_parametersLock)
        {
            _specsToAdd.Add(specimen);
            meter = GenerationMeter;
            simMeter = SimulationMeter;
        }
        if (meter == null) return;

        double newQuality = meter.MeasureQualityRecursive(specimen, null).Quality;
        double? newSimQuality = simMeter?.MeasureQualityRecursive(specimen, null).Quality;

        lock (_parametersLock)
        {
            bool better = BestEver == null || BestEverQuality < newQuality;
            if (!better && BestEver != null && BestEverQuality == newQuality && simMeter != null && newSimQuality.HasValue)
            {
                double oldSimQuality = simMeter.MeasureQualityRecursive(BestEver, null).Quality;
                better = oldSimQuality < newSimQuality.Value;
            }
            if (better)
            {
                BestEver = specimen;
                BestEverQuality = newQuality;
            }
        }
    }

    /// <summary>
    /// Snapshot of current generation specimens + queued additions + BestEver, deduplicated by reference.
    /// Used to seed external evaluators (e.g. GPU runner) from the simulation's current state.
    /// </summary>
    public List<TSpecimen> CollectSeedSpecimens()
    {
        List<TSpecimen> result = new List<TSpecimen>();
        lock (_parametersLock)
        {
            foreach (TSpecimen s in _specsToAdd)
                if (s != null) result.Add(s);
            foreach (TSpecimen s in _generation.Specimens)
                if (s != null && !result.Contains(s)) result.Add(s);
        }
        if (BestEver != null && !result.Contains(BestEver))
            result.Add(BestEver);
        return result;
    }
    public void Replace(TSpecimen spec)
    {
        lock (_parametersLock)
        {
            _specsToAdd.Add(spec);
            _clearSpecs = true;
            _reevaluateBest = true;
        }
    }

    private void SimulationTimer_Ticked(object sender, EventArgs e)
    {
        QualityMeter<TSpecimen> generationMeter;
        QualityMeter<TSpecimen> simulationMeter;
        Mutators<TSpecimen> mutators;
        ParentQueuer<TSpecimen> parentQueuer;
        bool reevaluateBest;
        int maxSpecimens;
        int generationMultiplier;
        lock (_parametersLock)
        {
            if (_clearSpecs)
            {
                _generation.Specimens.Clear();
                _clearSpecs = false;
            }
            _generation.Specimens.AddRange(_specsToAdd);
            _specsToAdd.Clear();
            generationMeter = GenerationMeter;
            simulationMeter = SimulationMeter;
            mutators = Mutators;
            parentQueuer = ParentQueuer;
            reevaluateBest = _reevaluateBest;
            _reevaluateBest = false;
            maxSpecimens = MaxSpecimens;
            generationMultiplier = GenerationMultiplier;
        }

        for (int index = 0; index < generationMultiplier; index++)
        {
            Generation<TSpecimen> newGeneration = _generation.CreateNextGeneration(mutators, generationMeter, maxSpecimens, parentQueuer);

            bool generationQualityImprovement = BestEverQuality < _generation.BestQuality;
            bool simulationQualityImprovement = simulationMeter != null &&
                                                BestEverQuality == _generation.BestQuality &&
                                                BestEver != null &&
                                                simulationMeter.MeasureQualityRecursive(BestEver, null).Quality <
                                                simulationMeter.MeasureQualityRecursive(_generation.Best, null).Quality;
            if (reevaluateBest || BestEver == null || generationQualityImprovement || simulationQualityImprovement)
            {
                reevaluateBest = false;
                if (_generation.Best.MutationDescription != null && generationQualityImprovement)
                {
                    string mutationDescription = string.Format("Gen:{0} Qual: ({1:F13} => {2:F13} - incr {3:F13})\n Mutation: {4}", GenerationNumber,
                        BestEverQuality, _generation.BestQuality, _generation.BestQuality - BestEverQuality, _generation.Best.MutationDescription.Text);
                    Log.Enqueue(mutationDescription);
                }
                BestEver = _generation.Best;
                BestEverQuality = _generation.BestQuality;
                if (OperatingSystem.IsWindows())
                {
                    if (generationQualityImprovement)
                        System.Media.SystemSounds.Asterisk.Play();
                    else
                        System.Media.SystemSounds.Beep.Play();
                }
            }
            GenerationNumber++;

            Generation<TSpecimen> prevGeneration = _generation;
            _generation = newGeneration;

            if (OnNextGenerationCreated(newGeneration, prevGeneration))
            {
                SimulationTimer.Pause();
                break;
            }
        }
    }
}

