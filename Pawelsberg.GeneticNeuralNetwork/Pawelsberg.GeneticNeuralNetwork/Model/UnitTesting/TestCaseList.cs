namespace Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting;

public class TestCaseList
{
    public List<TestCase> TestCases { get; set; }

    public TestCaseList()
    {
        TestCases = new List<TestCase>();
    }

    public static TestCaseList GetSimpleSumTestCaseList()
    {
        TestCase testCase1 = new TestCase { Inputs = new List<int> { 1, 1 }, Outputs = new List<int> { 2 } };
        TestCase testCase2 = new TestCase { Inputs = new List<int> { 2, 2 }, Outputs = new List<int> { 4 } };
        TestCase testCase3 = new TestCase { Inputs = new List<int> { 168952, -9658745 }, Outputs = new List<int> { -9489793 } };
        TestCaseList testCaseList = new TestCaseList { TestCases = new List<TestCase> { testCase1, testCase2, testCase3 } };
        return testCaseList;
        #region Result
        //Input 0 * 1.0 -> Neuron 0
        //Input 1 * 1.0 -> Neuron 0
        //Neuron 0 -> Output 0
        #endregion
    }
    public static TestCaseList GetRepeaterTestCaseList()
    {
        TestCase testCase1 = new TestCase { Inputs = new List<int> { 1 }, Outputs = new List<int> { 1 } };
        TestCase testCase2 = new TestCase { Inputs = new List<int> { 2 }, Outputs = new List<int> { 1, 1 } };
        TestCase testCase3 = new TestCase { Inputs = new List<int> { 10 }, Outputs = new List<int> { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 } };
        TestCaseList testCaseList = new TestCaseList { TestCases = new List<TestCase> { testCase1, testCase2, testCase3 } };
        return testCaseList;
        #region Result
        //    int v1 = 0;
        //    int v2 = 0;
        //    int v3 = 0;
        //    // program starts here
        //    v3 = Console.Skip();
        //    v1 = v3 / v3;
        //L4: Console.Write(v1);
        //    v2 = v1 / v3;
        //    v3 = v3 - v1;
        //    if (v2 == 0) goto L4;
        //    return;
        #endregion
    }
    public static TestCaseList GetCounterTestCaseList()
    {
        TestCase testCase1 = new TestCase { Inputs = new List<int> { 1 }, Outputs = new List<int> { 1 } };
        TestCase testCase2 = new TestCase { Inputs = new List<int> { 2 }, Outputs = new List<int> { 1, 2 } };
        TestCase testCase3 = new TestCase { Inputs = new List<int> { 10 }, Outputs = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 } };
        TestCaseList testCaseList = new TestCaseList { TestCases = new List<TestCase> { testCase1, testCase2, testCase3 } };
        return testCaseList;
        #region Result
        //    int v1 = 0;
        //    int v2 = 0;
        //    int v3 = 0;
        //    int v4 = 0;
        //L1: Console.Write(v1);
        //    v4 = v1 / v3;
        //    v1 = v1 + v2;
        //    if (v4 == 0) goto L1;
        //    return;
        //    // program starts here
        //    v1 = 1;
        //    v3 = int.Parse(Console.ReadLine());
        //    v2 = v1 * v1;
        //    goto L1;

        #endregion
    }
    public static TestCaseList GetSortingTestCaseList()
    {
        TestCase testCase1 = new TestCase { Inputs = new List<int> { 1, 1 }, Outputs = new List<int> { 1 } };
        TestCase testCase2 = new TestCase { Inputs = new List<int> { 2, 1, 2 }, Outputs = new List<int> { 1, 2 } };
        TestCase testCase3 = new TestCase { Inputs = new List<int> { 2, 2, 1 }, Outputs = new List<int> { 1, 2 } };
        TestCase testCase4 = new TestCase { Inputs = new List<int> { 3, 3, 2, 1 }, Outputs = new List<int> { 1, 2, 3 } };
        TestCase testCase5 = new TestCase { Inputs = new List<int> { 2, 13, 12 }, Outputs = new List<int> { 12, 13 } };
        TestCaseList testCaseList = new TestCaseList { TestCases = new List<TestCase> { testCase1, testCase2, testCase3, testCase4, testCase5 } };
        return testCaseList;
    }
    public static TestCaseList GetConstantTestCaseList()
    {
        TestCase testCase1 = new TestCase { Inputs = new List<int> { }, Outputs = new List<int> { 1, 13, 23, 43, 12, 32, 43 } };
        TestCaseList testCaseList = new TestCaseList { TestCases = new List<TestCase> { testCase1 } };
        return testCaseList;
        #region Result
        //int v1 = 0;
        //int v2 = 0;
        //int v3 = 0;
        //int v4 = 0;
        //// program starts here
        //v2 = 1;
        //Console.Write(v2);
        //v1 = 11;
        //v4 = 13;
        //v3 = 43;
        //v2 = 23;
        //Console.Write(v4);
        //Console.Write(v2);
        //v2 = v2 - v1;
        //Console.Write(v3);
        //Console.Write(v2);
        //v2 = v3 - v1;
        //Console.Write(v2);
        //Console.Write(v3);
        //return;
        //   quality -  41.38333
        #endregion
    }
}
