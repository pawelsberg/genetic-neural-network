using System.Text.Json;
using Pawelsberg.GeneticNeuralNetwork.Model;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking.DiskStoring;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGeneticsUnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting.DiskStoring;

namespace Pawelsberg.GeneticNeuralNetworkConsole.Model;

/// <summary>
/// Emits, between unique stdout markers, a single compact JSON document describing one
/// (test cases + network + propagations) combination: the network topology with stable
/// synapse indices, the program-accepted network text, and for every test case the
/// per-propagation synapse-potential snapshots plus pass/fail at 0.001 per output.
/// The web-export generator script drives this command over piped stdin and the comment
/// for each combination is injected by that script, not here.
/// </summary>
public class WebExportCommand : Command
{
    public static string Name = "webexport";

    // Mismatch tolerance per output value - same convention as ShowCommand/TestResultsMode.
    private const double c_goodDifference = 0.001d;
    private const string c_beginMarkerPrefix = "<<<WEBEXPORT begin ";
    private const string c_endMarker = "<<<WEBEXPORT end>>>";

    private string _testCasesName;
    private string _networkName;
    private int _propagations;

    public override void LoadParameters(CodedText text)
    {
        _testCasesName = text.ReadString();
        text.SkipWhiteCharacters();
        _networkName = text.ReadString();
        text.SkipWhiteCharacters();
        _propagations = text.ReadInt();
        text.SkipWhiteCharacters();
        if (!text.EOT)
            throw new Exception("Usage: webexport <testCases> <network> <propagations>");

        base.LoadParameters(text);
    }

    public override void Run(NetworkSimulation simulation)
    {
        TestCaseList testCaseList = TestCaseLists.LoadTestCaseList(_testCasesName);
        Network network = NetworkList.LoadNetwork(_networkName);

        List<Synapse> orderedSynapses = network.GetAllSynapses().ToList();

        List<object> nodeDtos = BuildNodeDtos(network);
        List<object> synapseDtos = BuildSynapseDtos(network, orderedSynapses);

        List<object> testCaseResults = new List<object>();
        bool solves = testCaseList.TestCases.Count > 0;
        foreach (TestCase testCase in testCaseList.TestCases)
        {
            TestCaseResult testCaseResult = RunTestCase(network, orderedSynapses, testCase);
            if (!testCaseResult.Pass)
                solves = false;
            testCaseResults.Add(new
            {
                inputs = testCase.Inputs,
                expected = testCase.Outputs,
                actual = testCaseResult.Actual,
                pass = testCaseResult.Pass,
                snapshots = testCaseResult.Snapshots
            });
        }

        object payload = new
        {
            testCases = _testCasesName,
            network = _networkName,
            propagations = _propagations,
            solves,
            networkText = network.ToText(),
            nodes = nodeDtos,
            synapses = synapseDtos,
            testCaseResults
        };

        string json = JsonSerializer.Serialize(payload);

        Console.ResetColor();
        Console.WriteLine($"{c_beginMarkerPrefix}testCases={_testCasesName} network={_networkName}>>>");
        Console.WriteLine(json);
        Console.WriteLine(c_endMarker);
    }

    private static List<object> BuildNodeDtos(Network network)
    {
        List<object> nodeDtos = new List<object>();
        for (int nodeIndex = 0; nodeIndex < network.Nodes.Count; nodeIndex++)
        {
            Node node = network.Nodes[nodeIndex];
            if (node is Neuron)
            {
                Neuron neuron = (Neuron)node;
                nodeDtos.Add(new { index = nodeIndex, type = "Neuron", activation = neuron.ActivationFunction.ToLetterCode() });
            }
            else if (node is Bias)
            {
                nodeDtos.Add(new { index = nodeIndex, type = "Bias", activation = (string)null });
            }
        }
        return nodeDtos;
    }

    private static List<object> BuildSynapseDtos(Network network, List<Synapse> orderedSynapses)
    {
        List<object> synapseDtos = new List<object>();
        for (int synapseIndex = 0; synapseIndex < orderedSynapses.Count; synapseIndex++)
        {
            Synapse synapse = orderedSynapses[synapseIndex];

            int inputIndex = network.Inputs.IndexOf(synapse);
            int outputIndex = network.Outputs.IndexOf(synapse);

            Node sourceNode = network.Nodes.FirstOrDefault(node => node.Outputs.Contains(synapse));
            Neuron destinationNeuron = network.Nodes.OfType<Neuron>().FirstOrDefault(neuron => neuron.Inputs.Contains(synapse));

            int? fromNodeIndex = sourceNode != null ? network.Nodes.IndexOf(sourceNode) : (int?)null;
            int? toNodeIndex = destinationNeuron != null ? network.Nodes.IndexOf(destinationNeuron) : (int?)null;
            double? multiplier = destinationNeuron != null
                ? destinationNeuron.InputMultiplier[destinationNeuron.Inputs.IndexOf(synapse)]
                : (double?)null;

            string role = inputIndex >= 0 ? "input" : outputIndex >= 0 ? "output" : "inner";

            synapseDtos.Add(new
            {
                index = synapseIndex,
                role,
                inputIndex = inputIndex >= 0 ? inputIndex : (int?)null,
                outputIndex = outputIndex >= 0 ? outputIndex : (int?)null,
                from = fromNodeIndex,
                to = toNodeIndex,
                multiplier
            });
        }
        return synapseDtos;
    }

    private TestCaseResult RunTestCase(Network network, List<Synapse> orderedSynapses, TestCase testCase)
    {
        if (network.Inputs.Count < testCase.Inputs.Count || network.Outputs.Count < testCase.Outputs.Count)
            return new TestCaseResult { Pass = false, Actual = new double[0], Snapshots = new double[0][] };

        RunningContext runningContext = new RunningContext();
        foreach (Synapse synapse in orderedSynapses)
            runningContext.SynapsePotentials.Add(synapse, 0d);
        for (int inputIndex = 0; inputIndex < testCase.Inputs.Count; inputIndex++)
            runningContext.SetPotential(network.Inputs[inputIndex], testCase.Inputs[inputIndex]);

        List<double[]> snapshots = new List<double[]>();
        snapshots.Add(CaptureSnapshot(runningContext, orderedSynapses));
        for (int step = 0; step < _propagations; step++)
        {
            network.SafeRun(runningContext, 1);
            snapshots.Add(CaptureSnapshot(runningContext, orderedSynapses));
        }

        double[] actual = new double[testCase.Outputs.Count];
        bool pass = true;
        for (int outputIndex = 0; outputIndex < testCase.Outputs.Count; outputIndex++)
        {
            double outputValue = SanitizeNumber(runningContext.GetPotential(network.Outputs[outputIndex]));
            actual[outputIndex] = outputValue;
            if (Math.Abs(testCase.Outputs[outputIndex] - outputValue) > c_goodDifference)
                pass = false;
        }

        return new TestCaseResult { Pass = pass, Actual = actual, Snapshots = snapshots.ToArray() };
    }

    private static double[] CaptureSnapshot(RunningContext runningContext, List<Synapse> orderedSynapses)
    {
        double[] snapshot = new double[orderedSynapses.Count];
        for (int synapseIndex = 0; synapseIndex < orderedSynapses.Count; synapseIndex++)
            snapshot[synapseIndex] = SanitizeNumber(runningContext.GetPotential(orderedSynapses[synapseIndex]));
        return snapshot;
    }

    // Non-finite potentials (a divergent network) cannot be represented in JSON; collapse
    // them to zero so one bad combination cannot abort the whole generation run.
    private static double SanitizeNumber(double value)
    {
        return double.IsFinite(value) ? value : 0d;
    }

    public override string ShortDescription
    {
        get
        {
            return "Emits JSON (between markers) for one test-cases/network/propagations\n" +
                   " combination, used by the web-export generator. Usage:\n" +
                   " webexport <testCases> <network> <propagations>";
        }
    }

    private class TestCaseResult
    {
        public bool Pass { get; set; }
        public double[] Actual { get; set; }
        public double[][] Snapshots { get; set; }
    }
}
