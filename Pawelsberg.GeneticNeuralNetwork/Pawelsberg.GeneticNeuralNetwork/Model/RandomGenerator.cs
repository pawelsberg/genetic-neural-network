using System;

namespace Pawelsberg.GeneticNeuralNetwork.Model
{
    public static class RandomGenerator
    {
        public static Random Random { get; set; }
        static RandomGenerator() { Random = new Random(); }

        public static double RandomValue()
        {
            if (Random.NextDouble() < 0.5d)
                return (Random.NextDouble() - 0.5d) * 4d;
            if (Random.NextDouble() < 0.5d)
                return (Random.NextDouble() - 0.5d) * 0.001d;
            /*  else if (Random.NextDouble() < 0.5d)
                 return (Random.NextDouble() - 0.5d)*0.000002d;
             else if (Random.NextDouble() < 0.5d)
                 return (Random.NextDouble() - 0.5d)*0.000000002d;
             else if (Random.NextDouble() < 0.5d)
               */
            return (Random.NextDouble() - 0.5d) * 20d;
            //else 
            //if (Random.NextDouble() < 0.5d)
            //    return (Random.NextDouble() - 0.5d)*0.000000000002d;
            //else 
            //    return (Random.NextDouble() - 0.5d)*2000d;
        }

        public static int RandomInteger()
        {
            if (Random.NextDouble() < 0.5d)
                return Random.Next(2) - 1;
            else if (Random.NextDouble() < 0.5d)
                return Random.Next(6) - 3;
            else if (Random.NextDouble() < 0.5d)
                return Random.Next(20) - 10;
            return Random.Next(100) - 50;
        }

        public static double Randomize(double value)
        {
            if (Random.NextDouble() < 0.5d)
            {
                return value + RandomValue();
            }

            double result = RandomValue();
            if (Random.NextDouble() < 0.5d)
                result += 1;
            return result;
        }

        public static void RunRandomTimes(Action action)
        {
            action();
            double random = Random.NextDouble();
            if (random > 0.33d)
                action();
            if (random > 0.66d)
                action();
            if (random > 0.85d)
                action();
            if (random > 0.95d)
                action();
            if (random > 0.99d)
                action();
        }

        public static void RunRandomTimes<T>(Action<T> action, T arg)
        {
            action(arg);
            double random = Random.NextDouble();
            if (random > 0.33d)
                action(arg);
            if (random > 0.66d)
                action(arg);
            if (random > 0.85d)
                action(arg);
            if (random > 0.95d)
                action(arg);
            if (random > 0.99d)
                action(arg);
        }

        public static int RandomizeInteger(int value)
        {
            if (Random.NextDouble() < 0.5d)
                return RandomInteger();
            else
                return value + RandomInteger();
        }
    }
}
