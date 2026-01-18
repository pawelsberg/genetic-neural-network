namespace Pawelsberg.GeneticNeuralNetwork.Model.Genetics;

public class QualityMeter<TSpecimen> where TSpecimen : ISpecimen
{
    public QualityMeter<TSpecimen> Parent { get; private set; }
    
    private List<QualityMeter<TSpecimen>> _children;
    public virtual List<QualityMeter<TSpecimen>>? Children 
    { 
        get
        {
            return _children;
        }
    }

    protected virtual double MaxMeterQuality { get { return 0d; } }
    public double? MaxQualityRecursive
    {
        get
        {
            List<QualityMeter<TSpecimen>>? children = Children;
            if (children == null)
                return null;

            double sum = MaxMeterQuality;
            foreach (QualityMeter<TSpecimen> child in children)
            {
                double? childMax = child.MaxQualityRecursive;
                if (childMax == null)
                    return null;
                sum += childMax.Value;
            }
            return sum;
        }
    }
    public QualityMeter(QualityMeter<TSpecimen> parent)
    {
        Parent = parent;
        _children = new List<QualityMeter<TSpecimen>>();
    }

    public virtual QualityMeasurement<TSpecimen> MeasureMeterQuality(TSpecimen spec, QualityMeasurement<TSpecimen> parentQualityMeasurement)
    {
        return new QualityMeasurement<TSpecimen>(this, parentQualityMeasurement);
    }
    public virtual QualityMeasurement<TSpecimen> MeasureQualityRecursive(TSpecimen spec, QualityMeasurement<TSpecimen> parentQualityMeasurement)
    {
        QualityMeasurement<TSpecimen> qualityMeasurement = MeasureMeterQuality(spec, parentQualityMeasurement);
        List<QualityMeter<TSpecimen>>? children = Children;
        if (children == null)
            throw new InvalidOperationException("Cannot measure quality when Children is null - meter not fully configured.");
        foreach (QualityMeter<TSpecimen> childMeter in children)
        {
            QualityMeasurement<TSpecimen> childQualityMeasurement = childMeter.MeasureQualityRecursive(spec, qualityMeasurement);
            qualityMeasurement.Children.Add(childQualityMeasurement);
            qualityMeasurement.Quality += childQualityMeasurement.Quality;
        }
        return qualityMeasurement;
    }
}


