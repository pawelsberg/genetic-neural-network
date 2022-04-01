namespace Pawelsberg.GeneticNeuralNetwork.Model.Genetics.QueuingParents;

public abstract class ParentQueuer<TSpecimen> where TSpecimen : ISpecimen
{
    public abstract IEnumerable<TSpecimen> QueueParents(Dictionary<TSpecimen, double> parentQualities);

    /// <summary> To be more fair - pick random with the same quality (not the first one) </summary>
    protected TSpecimen PickRandomWithSameQuality(Dictionary<TSpecimen, double> specQualities, double quality)
    {
        List<KeyValuePair<TSpecimen, double>> specQualitiesList = new List<KeyValuePair<TSpecimen, double>>(specQualities);
        int index;
        do
        {
            index = RandomGenerator.Random.Next(specQualities.Count);
        }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        while (specQualitiesList[index].Value != quality);
        return specQualitiesList[index].Key;
    }
}
