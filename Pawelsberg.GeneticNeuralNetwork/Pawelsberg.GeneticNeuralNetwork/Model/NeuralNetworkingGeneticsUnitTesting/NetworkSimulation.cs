using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics.QueuingParents;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics.Simulating;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.QualityMeasuring;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.Mutating;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting.QualityMeasuring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting;

public class NetworkSimulation : Simulation<Network>
{
    private int _maxNodes;
    private int _maxSynapses;
    private int _propagations;
    private TestCaseList _testCaseList;
    private ParentQueuerType _parentQueuerType;
    private Func<int, TestCaseList, QualityMeter<Network>> _qualityMeterFactory;

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
            UpdateGenerationMeterPropagations();
            UpdateMutators();
        }
    }
    public TestCaseList TestCaseList
    {
        get { return _testCaseList; }
        set
        {
            _testCaseList = value;
            UpdateGenerationMeterTestCaseList();
            UpdateMutators();
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
    public Func<int, TestCaseList, QualityMeter<Network>> QualityMeterFactory
    {
        get { return _qualityMeterFactory; }
        set
        {
            _qualityMeterFactory = value;
            RebuildGenerationMeter();
        }
    }

    public NetworkSimulation()
    {
        _maxNodes = 10;
        _maxSynapses = 30;
        _propagations = 4;
        _parentQueuerType = ParentQueuerType.Normal;
        NetworkMutators = NetworkMutators.CreateNormal(_maxNodes, _maxSynapses);

        _qualityMeterFactory = NetworkQualityMeters.CreateNormal;
        RebuildGenerationMeter();
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

    private void RebuildGenerationMeter()
    {
        GenerationMeter = _qualityMeterFactory(_propagations, _testCaseList);
    }

    private void UpdateGenerationMeterPropagations()
    {
        if (GenerationMeter is ITestCasesQualityMeterContainer container)
            container.Propagations = _propagations;
        else
            RebuildGenerationMeter();
    }

    private void UpdateGenerationMeterTestCaseList()
    {
        if (GenerationMeter is ITestCasesQualityMeterContainer container)
        {
            container.TestCaseList = _testCaseList;
            RecalculateMaxPossibleQuality();
        }
        else
            RebuildGenerationMeter();
    }

    private void OnNextGenerationCreated(object sender, NextGenerationCreatedEventArgs<Network> nextGenerationCreatedEventArgs)
    {
        if (GenerationNumber % 100 == 0 && BestEver != null)
            Add((Network)BestEver.DeepClone());
    }
}

