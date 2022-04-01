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
        double qualityForOneDiff = 0.01d;
        double qualityForGoodResult = 10d;
        double substractedQualityForOneDiff = 1d;
        double qualityForOneMs = 25d;
        double qualityForOneNode = 5d;
        double qualityForOneSynapse = 5d;
        double qualityForMultipSumEqOne = 0.0001d;

        QualityMeter<Network> rootMeter = new QualityMeter<Network>(null);
        if (testCaseList != null)
            foreach (TestCase testCase in testCaseList.TestCases)
            {
                TestCaseNetworkQualityMeter testCaseMeter = new TestCaseNetworkQualityMeter(rootMeter, testCase, propagations);
                testCaseMeter.Children.Add(new TestCaseDifferenceNetworkQualityMeter(testCaseMeter, goodDifference, qualityForOneDiff));
                testCaseMeter.Children.Add(new TestCaseGoodResultNetworkQualityMeter(testCaseMeter, goodDifference, qualityForGoodResult));
                testCaseMeter.Children.Add(new TestCaseSubstractErrorIfGoodNetworkQualityMeter(testCaseMeter, goodDifference,
                    substractedQualityForOneDiff));
                rootMeter.Children.Add(testCaseMeter);
            }

        QualityMeter<Network> ifAllGoodMeter = new TestCasesIfAllGoodNetworkQualityMeter(rootMeter, goodDifference);
        rootMeter.Children.Add(ifAllGoodMeter);

        ifAllGoodMeter.Children.Add(new TestCasesTotalTimeNetworkQualityMeter(ifAllGoodMeter, qualityForOneMs));
        ifAllGoodMeter.Children.Add(new TotalNodesNetworkQualityMeter(ifAllGoodMeter, qualityForOneNode));
        ifAllGoodMeter.Children.Add(new TotalSynapsesNetworkQualityMeter(ifAllGoodMeter, qualityForOneSynapse));
        ifAllGoodMeter.Children.Add(new MultiplierSumNetworkQualityMeter(ifAllGoodMeter, qualityForMultipSumEqOne));

        return rootMeter;
    }

    public static QualityMeter<Network> CreatePropagationsAgnostic(int propagations, TestCaseList testCaseList)
    {
        double goodDifference = 0.001d;
        double qualityForOneDiff = 0.01d;
        double qualityForGoodResult = 10d;
        double substractedQualityForOneDiff = 1d;
        double qualityForOneMs = 25d;
        double qualityForOneNode = 5d;
        double qualityForOneSynapse = 5d;
        double qualityForMultipSumEqOne = 0.0001d;
        int propagationValues = 1;
        double qualityForZeroLoops = 500d;

        QualityMeter<Network> rootMeter = new QualityMeter<Network>(null);
        if (testCaseList != null)
            foreach (int props in Enumerable.Range(propagations, propagationValues))
                foreach (TestCase testCase in testCaseList.TestCases)
                {
                    TestCaseNetworkQualityMeter testCaseMeter = new TestCaseNetworkQualityMeter(rootMeter, testCase, props);
                    testCaseMeter.Children.Add(new TestCaseDifferenceNetworkQualityMeter(testCaseMeter, goodDifference, qualityForOneDiff));
                    testCaseMeter.Children.Add(new TestCaseGoodResultNetworkQualityMeter(testCaseMeter, goodDifference, qualityForGoodResult));
                    testCaseMeter.Children.Add(new TestCaseSubstractErrorIfGoodNetworkQualityMeter(testCaseMeter, goodDifference,
                        substractedQualityForOneDiff));
                    rootMeter.Children.Add(testCaseMeter);
                }
        rootMeter.Children.Add(new NoLoopsNetworkQualityMeter(rootMeter, qualityForZeroLoops));
        QualityMeter<Network> ifAllGoodMeter = new TestCasesIfAllGoodNetworkQualityMeter(rootMeter, goodDifference);
        rootMeter.Children.Add(ifAllGoodMeter);

        ifAllGoodMeter.Children.Add(new TestCasesTotalTimeNetworkQualityMeter(ifAllGoodMeter, qualityForOneMs));
        ifAllGoodMeter.Children.Add(new TotalNodesNetworkQualityMeter(ifAllGoodMeter, qualityForOneNode));
        ifAllGoodMeter.Children.Add(new TotalSynapsesNetworkQualityMeter(ifAllGoodMeter, qualityForOneSynapse));
        ifAllGoodMeter.Children.Add(new MultiplierSumNetworkQualityMeter(ifAllGoodMeter, qualityForMultipSumEqOne));

        return rootMeter;
    }

    public static QualityMeter<Network> CreateLowestMultipliers(int propagations, TestCaseList testCaseList)
    {
        double goodDifference = 0.001d;
        double qualityForOneDiff = 0.01d;
        double qualityForGoodResult = 10d;
        double substractedQualityForOneDiff = 1d;
        //double qualityForOneMs = 25d;
        //double qualityForOneNode = 5d;
        //double qualityForOneSynapse = 5d;
        double qualityForMultipSumEqOne = 10.00d;

        QualityMeter<Network> rootMeter = new QualityMeter<Network>(null);
        if (testCaseList != null)
            foreach (TestCase testCase in testCaseList.TestCases)
            {
                TestCaseNetworkQualityMeter testCaseMeter = new TestCaseNetworkQualityMeter(rootMeter, testCase, propagations);
                testCaseMeter.Children.Add(new TestCaseDifferenceNetworkQualityMeter(testCaseMeter, goodDifference, qualityForOneDiff));
                testCaseMeter.Children.Add(new TestCaseGoodResultNetworkQualityMeter(testCaseMeter, goodDifference, qualityForGoodResult));
                testCaseMeter.Children.Add(new TestCaseSubstractErrorIfGoodNetworkQualityMeter(testCaseMeter, goodDifference,
                    substractedQualityForOneDiff));
                rootMeter.Children.Add(testCaseMeter);
            }

        rootMeter.Children.Add(new MultiplierSumNetworkQualityMeter(rootMeter, qualityForMultipSumEqOne));

        return rootMeter;
    }

    public static QualityMeter<Network> CreateSequential(int propagations, TestCaseList testCaseList)
    {
        double goodDifference = 0.001d;
        double qualityForOneDiff = 0.01d;
        double qualityForGoodResult = 10d;
        double substractedQualityForOneDiff = 1d;

        QualityMeter<Network> rootMeter = new QualityMeter<Network>(null);
        QualityMeter<Network> parentMeter = rootMeter;
        if (testCaseList != null)
            foreach (TestCase testCase in testCaseList.TestCases)
            {
                TestCaseNetworkQualityMeter testCaseMeter = new TestCaseNetworkQualityMeter(parentMeter, testCase, propagations);
                testCaseMeter.Children.Add(new TestCaseDifferenceNetworkQualityMeter(testCaseMeter, goodDifference, qualityForOneDiff));
                testCaseMeter.Children.Add(new TestCaseGoodResultNetworkQualityMeter(testCaseMeter, goodDifference, qualityForGoodResult));
                testCaseMeter.Children.Add(new TestCaseSubstractErrorIfGoodNetworkQualityMeter(testCaseMeter, goodDifference,
                    substractedQualityForOneDiff));
                parentMeter.Children.Add(testCaseMeter);

                QualityMeter<Network> ifAllGoodMeter = new TestCasesIfAllGoodNetworkQualityMeter(parentMeter, goodDifference);
                parentMeter.Children.Add(ifAllGoodMeter);
                parentMeter = ifAllGoodMeter;
            }
        return rootMeter;
    }

    public static QualityMeter<Network> CreateAggregate(int propagations, TestCaseList testCaseList)
    {
        QualityMeter<Network> rootMeter = new TestCaseListNetworkQualityMeter(null, testCaseList, propagations);
        return rootMeter;
    }
}
