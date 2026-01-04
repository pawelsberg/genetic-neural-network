using System.Xml;

namespace Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting.DiskStoring;

public static class TestCaseListExtension
{
    private const string c_xmlTestCaseList = "TestCaseList";

    public static TestCaseList CreateFromXml(XmlReader reader)
    {
        TestCaseList result = new TestCaseList();
        reader.ReadToFollowing(c_xmlTestCaseList);
        bool emptyTestCase = reader.IsEmptyElement;
        reader.ReadStartElement(c_xmlTestCaseList);
        while (reader.IsStartElement())
            result.TestCases.Add(TestCaseExtension.Load(reader));
        if (!emptyTestCase)
            reader.ReadEndElement(); // c_xmlTestCaseList
        return result;
    }

    public static void Save(this TestCaseList thisTestCaseList, string fileName)
    {
        XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
        xmlWriterSettings.Indent = true;
        XmlWriter writer = XmlWriter.Create(fileName, xmlWriterSettings);
        writer.WriteStartElement(c_xmlTestCaseList);
        foreach (TestCase testCase in thisTestCaseList.TestCases)
            testCase.Save(writer);

        writer.WriteEndElement(); // TestCaseList
        writer.Close();
    }

    public static TestCaseList Load(string fileName)
    {
        XmlReader reader = XmlReader.Create(fileName);
        TestCaseList result = CreateFromXml(reader);
        reader.Close();
        return result;
    }
}
