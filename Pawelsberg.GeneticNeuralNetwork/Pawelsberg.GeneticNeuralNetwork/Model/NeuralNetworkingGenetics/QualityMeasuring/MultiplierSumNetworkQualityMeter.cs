using System;
using System.Linq;
using Pawelsberg.GeneticNeuralNetwork.Model.Genetics;
using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingGenetics.QualityMeasuring
{

    public class MultiplierSumNetworkQualityMeter : QualityMeter<Network>
    {
        public double QualityForSumEqOne { get; set; }

        public MultiplierSumNetworkQualityMeter(QualityMeter<Network> parent, double qualityForSumEqOne) : base(parent)
        {
            QualityForSumEqOne = qualityForSumEqOne;
        }

        public override QualityMeasurement<Network> MeasureMeterQuality(Network network, QualityMeasurement<Network> parentQualityMeasurement)
        {
            QualityMeasurement<Network> result = new QualityMeasurement<Network>(this, parentQualityMeasurement);

            double multiplierSum = 0;
            foreach (Neuron neuron in network.Nodes.OfType<Neuron>())
            {
                double sum = 0;
                foreach (double mltp in neuron.InputMultiplier)
                    sum += Math.Abs(mltp);
                multiplierSum += sum;
            }
            result.Quality = QualityForSumEqOne / (1d + multiplierSum); // smaller sum of multipliers than better
            return result;
        }
        protected override double MaxMeterQuality
        {
            get
            {
                return QualityForSumEqOne;
            }
        }
    }

}
