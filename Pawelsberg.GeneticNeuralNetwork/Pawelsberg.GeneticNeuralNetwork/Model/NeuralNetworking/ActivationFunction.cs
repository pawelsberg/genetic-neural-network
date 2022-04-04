namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;

public enum ActivationFunction
{
    Linear,
    Threshold,
    Squashing
}
public static class ActivationFunctionExtension
{
    public static string ToLetterCode(this ActivationFunction thisActivationFunction)
    {
        switch (thisActivationFunction)
        {
            default:
            case ActivationFunction.Linear: return "L";
            case ActivationFunction.Threshold: return "T";
            case ActivationFunction.Squashing: return "S";
        }
    }
    public static double Apply(this ActivationFunction thisActivationFunction, double outputPotential)
    {
        switch (thisActivationFunction)
        {
            default:
            case ActivationFunction.Linear:
                return outputPotential;
            case ActivationFunction.Threshold:
                // threshold activation function limits output to the range from -1 to 1
                return Math.Min(Math.Max(-1d, outputPotential), 1d);
            case ActivationFunction.Squashing:
                // threshold activation function limits output to the range from 0 to 1
                return Math.Min(Math.Max(0d, outputPotential), 1d);
        }
    }
    public static double ApplyDerivative(this ActivationFunction thisActivationFunction, double outputPotential)
    {
        switch (thisActivationFunction)
        {
            default:
            case ActivationFunction.Linear:
                return 1;
            case ActivationFunction.Threshold:
                // derivative of the threshold activation function is input in range from -1 to 1 zero otherwise
                return outputPotential >= -1 && outputPotential <= 1 ? 1 : 0; 
            case ActivationFunction.Squashing:
                // derivative of the threshold activation function is input in range from 0 to 1 zero otherwise
                return outputPotential >= 0 && outputPotential <= 1 ? 1 : 0;
        }
    }
    public static ActivationFunction FromLetterCode(string letterCode)
    {
        switch (letterCode)
        {
            default:
            case "L": return ActivationFunction.Linear;
            case "T": return ActivationFunction.Threshold;
            case "S": return ActivationFunction.Squashing;
        }
    }
    public static double Derivative(this ActivationFunction thisActivationFunction, double value)
    {
        switch (thisActivationFunction)
        {
            default:
            case ActivationFunction.Linear:
                return 1;
            case ActivationFunction.Threshold:
                return value < -1 || value > 1 ? 0 : 1;
            case ActivationFunction.Squashing:
                return value < 0 || value > 1 ? 0 : 1;
        }
    }
}

