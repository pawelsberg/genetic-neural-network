using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.QualityMeasuring;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;
using Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting.DiskStoring;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuranNetworkingGeneticsUnitTesting.QualityMeasuring;

public class NetworkQualityMeters
{
    public static QualityMeter<Network> CreateNormal(int propagations, TestCaseList testCaseList)
    {
        double goodDifference = 0.001d;
        double qualityForOneMs = 25d;
        double qualityForOneNode = 5d;
        double qualityForOneSynapse = 5d;
        double qualityForMultipSumEqOne = 0.0001d;

        Func<QualityMeter<Network>, TestCase, int, TestCaseNetworkQualityMeter> factory = (parent, testCase, props) =>
        {
            double qualityForOneDiff = 0.01d;
            double qualityForGoodResult = 10d;
            double substractedQualityForOneDiff = 1d;

            var testCaseMeter = new TestCaseNetworkQualityMeter(parent, testCase, props);
            testCaseMeter.Children.Add(new TestCaseDifferenceNetworkQualityMeter(testCaseMeter, goodDifference, qualityForOneDiff));
            testCaseMeter.Children.Add(new TestCaseGoodResultNetworkQualityMeter(testCaseMeter, goodDifference, qualityForGoodResult));
            testCaseMeter.Children.Add(new TestCaseSubstractErrorIfGoodNetworkQualityMeter(testCaseMeter, goodDifference, substractedQualityForOneDiff));
            return testCaseMeter;
        };

        var testCasesContainer = new TestCasesContainerQualityMeter(testCaseList, propagations, factory);

        QualityMeter<Network> ifAllGoodMeter = new TestCasesIfAllGoodNetworkQualityMeter(testCasesContainer, goodDifference);
        testCasesContainer.AddStaticChild(ifAllGoodMeter);

        ifAllGoodMeter.Children.Add(new TestCasesTotalTimeNetworkQualityMeter(ifAllGoodMeter, qualityForOneMs));
        ifAllGoodMeter.Children.Add(new TotalNodesNetworkQualityMeter(ifAllGoodMeter, qualityForOneNode));
        ifAllGoodMeter.Children.Add(new TotalSynapsesNetworkQualityMeter(ifAllGoodMeter, qualityForOneSynapse));
        ifAllGoodMeter.Children.Add(new MultiplierSumNetworkQualityMeter(ifAllGoodMeter, qualityForMultipSumEqOne));

        return testCasesContainer;
    }

    public static QualityMeter<Network> CreatePropagationsAgnostic(int propagationsFrom, int propagationsTo, TestCaseList testCaseList)
    {
        double goodDifference = 0.001d;
        double qualityForOneMs = 25d;
        double qualityForOneNode = 5d;
        double qualityForOneSynapse = 5d;
        double qualityForMultipSumEqOne = 0.0001d;
        double qualityForZeroLoops = 500d;

        Func<QualityMeter<Network>, TestCase, int, TestCaseNetworkQualityMeter> factory = (parent, testCase, props) =>
        {
            double qualityForOneDiff = 0.01d;
            double qualityForGoodResult = 10d;
            double substractedQualityForOneDiff = 1d;

            var testCaseMeter = new TestCaseNetworkQualityMeter(parent, testCase, props);
            testCaseMeter.Children.Add(new TestCaseDifferenceNetworkQualityMeter(testCaseMeter, goodDifference, qualityForOneDiff));
            testCaseMeter.Children.Add(new TestCaseGoodResultNetworkQualityMeter(testCaseMeter, goodDifference, qualityForGoodResult));
            testCaseMeter.Children.Add(new TestCaseSubstractErrorIfGoodNetworkQualityMeter(testCaseMeter, goodDifference, substractedQualityForOneDiff));
            return testCaseMeter;
        };

        var testCasesContainer = new TestCasesContainerQualityMeter(testCaseList, propagationsFrom, propagationsTo, factory);

        testCasesContainer.AddStaticChild(new NoLoopsNetworkQualityMeter(testCasesContainer, qualityForZeroLoops));
        QualityMeter<Network> ifAllGoodMeter = new TestCasesIfAllGoodNetworkQualityMeter(testCasesContainer, goodDifference);
        testCasesContainer.AddStaticChild(ifAllGoodMeter);

        ifAllGoodMeter.Children.Add(new TestCasesTotalTimeNetworkQualityMeter(ifAllGoodMeter, qualityForOneMs));
        ifAllGoodMeter.Children.Add(new TotalNodesNetworkQualityMeter(ifAllGoodMeter, qualityForOneNode));
        ifAllGoodMeter.Children.Add(new TotalSynapsesNetworkQualityMeter(ifAllGoodMeter, qualityForOneSynapse));
        ifAllGoodMeter.Children.Add(new MultiplierSumNetworkQualityMeter(ifAllGoodMeter, qualityForMultipSumEqOne));

        return testCasesContainer;
    }

    public static QualityMeter<Network> CreateLowestMultipliers(int propagations, TestCaseList testCaseList)
    {
        double goodDifference = 0.001d;
        double qualityForMultipSumEqOne = 10.00d;

        Func<QualityMeter<Network>, TestCase, int, TestCaseNetworkQualityMeter> factory = (parent, testCase, props) =>
        {
            double qualityForOneDiff = 0.01d;
            double qualityForGoodResult = 10d;
            double substractedQualityForOneDiff = 1d;

            var testCaseMeter = new TestCaseNetworkQualityMeter(parent, testCase, props);
            testCaseMeter.Children.Add(new TestCaseDifferenceNetworkQualityMeter(testCaseMeter, goodDifference, qualityForOneDiff));
            testCaseMeter.Children.Add(new TestCaseGoodResultNetworkQualityMeter(testCaseMeter, goodDifference, qualityForGoodResult));
            testCaseMeter.Children.Add(new TestCaseSubstractErrorIfGoodNetworkQualityMeter(testCaseMeter, goodDifference, substractedQualityForOneDiff));
            return testCaseMeter;
        };

        var testCasesContainer = new TestCasesContainerQualityMeter(testCaseList, propagations, factory);

        testCasesContainer.AddStaticChild(new MultiplierSumNetworkQualityMeter(testCasesContainer, qualityForMultipSumEqOne));

        return testCasesContainer;
    }

    public static QualityMeter<Network> CreateSequential(int propagations, TestCaseList testCaseList)
    {
        double goodDifference = 0.001d;

        Func<QualityMeter<Network>, TestCase, int, TestCaseNetworkQualityMeter> factory = (parent, testCase, props) =>
        {
            double qualityForOneDiff = 0.01d;
            double qualityForGoodResult = 10d;
            double substractedQualityForOneDiff = 1d;

            var testCaseMeter = new TestCaseNetworkQualityMeter(parent, testCase, props);
            testCaseMeter.Children.Add(new TestCaseDifferenceNetworkQualityMeter(testCaseMeter, goodDifference, qualityForOneDiff));
            testCaseMeter.Children.Add(new TestCaseGoodResultNetworkQualityMeter(testCaseMeter, goodDifference, qualityForGoodResult));
            testCaseMeter.Children.Add(new TestCaseSubstractErrorIfGoodNetworkQualityMeter(testCaseMeter, goodDifference, substractedQualityForOneDiff));
            return testCaseMeter;
        };

        var testCasesContainer = new TestCasesSequentialContainerQualityMeter(testCaseList, propagations, goodDifference, factory);

        return testCasesContainer;
    }

    public static QualityMeter<Network> CreateAggregate(int propagations, TestCaseList testCaseList)
    {
        QualityMeter<Network> rootMeter = new TestCaseListNetworkQualityMeter(null, testCaseList, propagations);
        return rootMeter;
    }

}
