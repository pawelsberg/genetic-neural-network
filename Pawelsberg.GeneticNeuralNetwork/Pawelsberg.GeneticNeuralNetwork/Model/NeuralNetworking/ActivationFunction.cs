namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;

public enum ActivationFunction
{
    Linear,
    Threshold,
    Squashing,
    Sigmoid,
    Tanh
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
            case ActivationFunction.Sigmoid: return "Σ";
            case ActivationFunction.Tanh: return "H";
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
            case ActivationFunction.Sigmoid:
                // sigmoid activation function limits output to the range from 0 to 1
                return 1 / (1 + Math.Exp(-outputPotential));
            case ActivationFunction.Tanh:
                // tanh activation function limits output to the range from -1 to 1
                return Math.Tanh(outputPotential);
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
            case ActivationFunction.Sigmoid:
                // derivative of the sigmoid activation function is sigmoid(x) * (1 - sigmoid(x))
                return thisActivationFunction.Apply(outputPotential) * (1 - thisActivationFunction.Apply(outputPotential));
            case ActivationFunction.Tanh:
                // derivative of the tanh activation function is 1 - tanh(x)^2
                return 1 - Math.Pow(thisActivationFunction.Apply(outputPotential), 2);
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
            case "Σ": return ActivationFunction.Sigmoid;
            case "H": return ActivationFunction.Tanh;
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
            case ActivationFunction.Sigmoid:
                return thisActivationFunction.Apply(value) * (1 - thisActivationFunction.Apply(value));
            case ActivationFunction.Tanh:
                return 1 - Math.Pow(thisActivationFunction.Apply(value), 2);
        }
    }
}

