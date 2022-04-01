namespace Pawelsberg.GeneticNeuralNetwork.Model.Genetics.QueuingParents;

public class RandomEndParentQueuer<TSpecimen> : ParentQueuer<TSpecimen> where TSpecimen : ISpecimen
{
    public override IEnumerable<TSpecimen> QueueParents(Dictionary<TSpecimen, double> parentQualities)
    {
        // 
        int sortedCount = (parentQualities.Count + 1) / 2;
        List<TSpecimen> sortedList = parentQualities.OrderByDescending(nQ => nQ.Value).Take(sortedCount).Select(kvp => kvp.Key).ToList();
        List<TSpecimen> endList = parentQualities.Select(kvp => kvp.Key).Where(net => !sortedList.Contains(net)).ToList();
        List<TSpecimen> randomizedEndList = new List<TSpecimen>();

        foreach (TSpecimen specimen in endList)
        {
            randomizedEndList.Insert(RandomGenerator.Random.Next(randomizedEndList.Count), specimen);
        }

        for (int i = 0; i < Math.Max(sortedList.Count, endList.Count); i++)
        {
            if (i < sortedList.Count)
                yield return sortedList[i];
            if (i < endList.Count)
                yield return endList[i];
        }
    }
}
