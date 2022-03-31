namespace Pawelsberg.GeneticNeuralNetwork.Model.Genetics
{

    public class QualityMeasurement<TSpecimen> where TSpecimen : ISpecimen
    {
        public double Quality { get; set; }
        public QualityMeasurement<TSpecimen> Parent { get; private set; }
        public List<QualityMeasurement<TSpecimen>> Children { get; private set; }
        public QualityMeter<TSpecimen> Meter { get; private set; }
        public QualityMeasurement(QualityMeter<TSpecimen> meter, QualityMeasurement<TSpecimen> parent)
        {
            Children = new List<QualityMeasurement<TSpecimen>>();
            Meter = meter;
            Parent = parent;
        }
    }


}
