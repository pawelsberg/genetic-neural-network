using System.Globalization;
using System.Xml;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting;

namespace Pawelsberg.GeneticNeuralNetwork.Model.UnitTesting.DiskStoring
{
    public static class TestCaseExtension
    {
        private const string c_xmlTestCase = "TestCase";
        private const string c_xmlInput = "Input";
        private const string c_xmlOutput = "Output";

        public static TestCase Load(XmlReader reader)
        {
            TestCase result = new TestCase();
            bool emptyTestCase = reader.IsEmptyElement;
            reader.ReadStartElement(c_xmlTestCase);
            while (reader.IsStartElement())
                switch (reader.Name)
                {
                    case c_xmlInput:
                        result.Inputs.Add(int.Parse(reader.ReadElementString(c_xmlInput)));
                        break;
                    case c_xmlOutput:
                        result.Outputs.Add(int.Parse(reader.ReadElementString(c_xmlOutput)));
                        break;
                }
            if (!emptyTestCase)
                reader.ReadEndElement(); // c_xmlTestCase
            return result;
        }
        public static void Save(this TestCase thisTestCase, XmlWriter writer)
        {
            writer.WriteStartElement(c_xmlTestCase);
            foreach (int input in thisTestCase.Inputs)
                writer.WriteElementString(c_xmlInput, input.ToString(CultureInfo.InvariantCulture));
            foreach (int output in thisTestCase.Outputs)
                writer.WriteElementString(c_xmlOutput, output.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();//TestCase
        }
    }
}
