namespace Pawelsberg.GeneticNeuralNetwork.Model.Genetics.QueuingParents;

public class SimpleParentQueuer<TSpecimen> : ParentQueuer<TSpecimen> where TSpecimen : ISpecimen
{
    public override IEnumerable<TSpecimen> QueueParents(Dictionary<TSpecimen, double> parentQualities)
    {
        foreach (KeyValuePair<TSpecimen, double> specimenQuality in parentQualities.OrderByDescending(nQ => nQ.Value))
        {
            TSpecimen parent = PickRandomWithSameQuality(parentQualities, specimenQuality.Value);
            yield return parent;
        }
    }
}
