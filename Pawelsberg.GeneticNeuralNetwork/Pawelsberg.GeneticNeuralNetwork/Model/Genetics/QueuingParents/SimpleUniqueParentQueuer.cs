namespace Pawelsberg.GeneticNeuralNetwork.Model.Genetics.QueuingParents;

public class SimpleUniqueParentQueuer<TSpecimen> : ParentQueuer<TSpecimen> where TSpecimen : ISpecimen
{
    public override IEnumerable<TSpecimen> QueueParents(Dictionary<TSpecimen, double> parentQualities)
    {
        List<TSpecimen> resultList = new List<TSpecimen>();
        foreach (KeyValuePair<TSpecimen, double> specimenQuality in parentQualities.OrderByDescending(nQ => nQ.Value))
        {
            TSpecimen parent;
            do
            {
                parent = PickRandomWithSameQuality(parentQualities, specimenQuality.Value);
            } while (resultList.Contains(parent));
            resultList.Add(parent);
            yield return parent;
        }
    }
}
