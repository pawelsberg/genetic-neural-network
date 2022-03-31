using Pawelsberg.GeneticNeuralNetwork.Model.Genetics.QueuingParents;

namespace Pawelsberg.GeneticNeuralNetwork.Model.Genetics
{
    public class Generation<TSpecimen> where TSpecimen : ISpecimen
    {
        public List<TSpecimen> Specimens { get; set; }

        public TSpecimen Best { get; private set; }
        public double BestQuality { get; private set; }
        public double AvrageQuality { get; private set; }

        public Generation() { Specimens = new List<TSpecimen>(); }

        public Generation<TSpecimen> CreateNextGeneration(Mutators<TSpecimen> mutators, QualityMeter<TSpecimen> meter, int maxSpecs, ParentQueuer<TSpecimen> parentQueuer)
        {
            Generation<TSpecimen> nextGeneration = new Generation<TSpecimen>();

            // measure quality of specimens
            Dictionary<TSpecimen, double> specQualities = new Dictionary<TSpecimen, double>();
            foreach (TSpecimen spec in Specimens)
            {
                QualityMeasurement<TSpecimen> qualityMeasurement = meter.MeasureQualityRecursive(spec, null);
                specQualities.Add(spec, qualityMeasurement.Quality);
            }
            AvrageQuality = specQualities.Average(nQ => nQ.Value);

            // TODO - separate children policy
            // create children for best specimens
            bool wasBest = false;
            foreach (TSpecimen parent in parentQueuer.QueueParents(specQualities))
            {
                // for the best we want also not modified clone
                if (!wasBest)
                {
                    TSpecimen bestCloneChild = (TSpecimen)parent.DeepClone();
                    bestCloneChild.MutationDescription = new MutationDescription() { Text = "No mutations" };
                    nextGeneration.Specimens.Add(bestCloneChild);
                    BestQuality = specQualities[parent];
                    Best = parent;
                    wasBest = true;
                }

                // two children for each good network - if there is space left
                if (nextGeneration.Specimens.Count >= maxSpecs)
                    break;
                TSpecimen childOne = (TSpecimen)parent.DeepClone();
                childOne.MutationDescription = mutators.Mutate(childOne);
                nextGeneration.Specimens.Add(childOne);
                if (nextGeneration.Specimens.Count >= maxSpecs)
                    break;
                TSpecimen childTwo = (TSpecimen)parent.DeepClone();
                childTwo.MutationDescription = mutators.Mutate(childTwo);
                nextGeneration.Specimens.Add(childTwo);
            }

            return nextGeneration;
        }

    }
}
