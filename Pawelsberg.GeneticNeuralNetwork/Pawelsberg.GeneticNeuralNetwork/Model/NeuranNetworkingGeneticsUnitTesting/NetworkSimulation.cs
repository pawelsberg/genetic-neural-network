using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics.QueuingParents;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics.Simulating;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.QualityMeasuring;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting.Mutating;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting.QualityMeasuring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting.DiskStoring;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting;

public class NetworkSimulation : Simulation<Network>
{
    private int _maxNodes;
    private int _maxSynapses;
    private int _propagations;
    private TestCaseList _testCaseList;
    private MeterType _meterType;
    private ParentQueuerType _parentQueuerType;

    public int MaxNodes
    {
        get { return _maxNodes; }
        set
        {
            _maxNodes = value;
            UpdateMutators();
        }
    }
    public int MaxSynapses
    {
        get { return _maxSynapses; }
        set
        {
            _maxSynapses = value;
            UpdateMutators();
        }
    }
    public int Propagations
    {
        get { return _propagations; }
        set
        {
            _propagations = value;
            UpdateGenerationMeter();
            UpdateMutators();
        }
    }
    public TestCaseList TestCaseList
    {
        get { return _testCaseList; }
        set
        {
            _testCaseList = value;
            UpdateGenerationMeter();
            UpdateMutators();
        }
    }
    public MeterType MeterType
    {
        get { return _meterType; }
        set
        {
            _meterType = value;
            UpdateGenerationMeter();
        }
    }
    public ParentQueuerType ParentQueuerType
    {
        get { return _parentQueuerType; }
        set
        {
            _parentQueuerType = value;
            UpdateParentQueuer();
        }
    }
    public NetworkMutators NetworkMutators
    {
        get { return Mutators as NetworkMutators; }
        set
        {
            Mutators = value;
            UpdateMutators();
        }
    }

    public NetworkSimulation()
    {
        _maxNodes = 10;
        _maxSynapses = 30;
        _propagations = 4;
        _meterType = MeterType.Normal;
        _parentQueuerType = ParentQueuerType.Normal;
        NetworkMutators = NetworkMutators.CreateNormal(_maxNodes, _maxSynapses);

        UpdateGenerationMeter();
        CreateSimulationMeter();
        UpdateMutators();
        UpdateParentQueuer();
        Network network = Network.CreateSimplest(1, 1);
        Add(network);

        NextGenerationCreated += OnNextGenerationCreated;
    }

    private void UpdateParentQueuer()
    {
        switch (_parentQueuerType)
        {
            case ParentQueuerType.Normal: ParentQueuer = new SimpleParentQueuer<Network>(); break;
            case ParentQueuerType.Unique: ParentQueuer = new SimpleUniqueParentQueuer<Network>(); break;
            case ParentQueuerType.RandomEnd: ParentQueuer = new RandomEndParentQueuer<Network>(); break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void UpdateMutators()
    {
        NetworkMutators.UpdateParameters(_maxNodes, _maxSynapses, _propagations, _testCaseList);
    }

    private void CreateSimulationMeter()
    {
        double qualityForOneNode = 1d;
        double qualityForOneSynapse = 1d;
        QualityMeter<Network> baseMeter = new QualityMeter<Network>(null);
        baseMeter.Children.Add(new TotalNodesNetworkQualityMeter(baseMeter, qualityForOneNode));
        baseMeter.Children.Add(new TotalSynapsesNetworkQualityMeter(baseMeter, qualityForOneSynapse));
        SimulationMeter = baseMeter;
    }
    private void UpdateGenerationMeter()
    {
        switch (_meterType)
        {
            case MeterType.Normal:
                GenerationMeter = NetworkQualityMeters.CreateNormal(_propagations, _testCaseList);
                break;
            case MeterType.PropagationsAgnostic:
                GenerationMeter = NetworkQualityMeters.CreatePropagationsAgnostic(_propagations, _testCaseList);
                break;
            case MeterType.LowestMultipliers:
                GenerationMeter = NetworkQualityMeters.CreateLowestMultipliers(_propagations, _testCaseList);
                break;
            case MeterType.Sequential:
                GenerationMeter = NetworkQualityMeters.CreateSequential(_propagations, _testCaseList);
                break;
            case MeterType.Aggregate:
                GenerationMeter = NetworkQualityMeters.CreateAggregate(_propagations, _testCaseList);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnNextGenerationCreated(object sender, NextGenerationCreatedEventArgs<Network> nextGenerationCreatedEventArgs)
    {
        if (GenerationNumber % 100 == 0 && BestEver != null)
            Add((Network)BestEver.DeepClone());
    }
}

