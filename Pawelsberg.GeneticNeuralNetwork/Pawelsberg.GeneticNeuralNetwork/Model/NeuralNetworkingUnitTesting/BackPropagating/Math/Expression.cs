using Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworking;

namespace Pawelsberg.GeneticNeuralNetwork.Model.NeuralNetworkingUnitTesting.BackPropagating.Math;

public abstract class Expression
{
    public abstract double Value { get; set; }
    public virtual void Calc() { }
    public virtual List<Expression> Expressions { get; set; } = new List<Expression>();
    public IEnumerable<Expression> GetExpressionsRecursive()
    {
        return Expressions.SelectMany(e => e.GetExpressionsRecursive()).Concat(new List<Expression> { this });
    }
    public virtual Expression DerivativeOver(Multiplier multiplierExpression)
    {
        return new Constant { Value = 0 };
    }
    public virtual Expression Optimised()
    {
        return DeepClone();
    }
    public abstract Expression DeepClone();
}
public class Constant : Expression
{
    public override double Value { get; set; }
    public override Expression DeepClone()
    {
        return new Constant { Value = Value };
    }
    public override string ToString()
    {
        return $"(C:{Value})";
    }
}
public class Input : Expression
{
    public Synapse InputSynapse { get; set; }
    public override double Value { get; set; }

    public override Expression DeepClone()
    {
        return new Input { InputSynapse = InputSynapse, Value = Value };
    }
    public override string ToString()
    {
        return $"(I:{Value})";
    }
}
public class Multiplier : Expression
{
    public Synapse Synapse { get; set; }
    public override double Value { get; set; }

    public override Expression DeepClone()
    {
        return new Multiplier { Synapse = Synapse, Value = Value };
    }
    public override Expression DerivativeOver(Multiplier multiplierExpression)
    {
        var result = new Constant { Value = multiplierExpression.Synapse == Synapse ? 1 : 0 };
        return result;
    }
    public override string ToString()
    {
        return $"(M:{Value})";
    }
}
public class MultiplyOperation : Expression
{
    public override List<Expression> Expressions { get; set; } = new List<Expression>();
    public override double Value { get; set; }
    public override void Calc()
    {
        Expressions.ForEach(e => e.Calc());
        Value = Expressions.Select(e => e.Value).Aggregate((e1, e2) => e1 * e2);
    }

    public override Expression Optimised()
    {
        if (Expressions.Any(expr => expr is Constant && (expr as Constant).Value == 0))
        { // Multiply by zero is zero
            return new Constant() { Value = 0 };
        }
        else
        {
            List<Expression> optimisedExpressions = Expressions.Select(expr => expr.Optimised()).ToList();
            if (optimisedExpressions.Any(expr => expr is Constant && (expr as Constant).Value == 0))
            { // Multiply by zero in optimised is zero
                return new Constant() { Value = 0 };
            }
            else
            {
                List<Constant> constants = optimisedExpressions.Where(e => e is Constant).Cast<Constant>().ToList();
                double constVal = 1; // multiplication of all constants
                constants.ForEach(c => constVal = constVal * c.Value);
                List<Expression> nonConstants = optimisedExpressions.Where(e => !constants.Contains(e)).Select(e => e.DeepClone()).ToList();
                // TODO: more possible optimisations - mul of mul 
                //                                   - mull of the same thing twice    2 x thing
                if (constants.Count > 0 && nonConstants.Count == 0)
                    return new Constant { Value = constVal }; // if there are only constants - combine them into one
                else if (constants.Count == 0 && nonConstants.Count == 1 || constants.Count > 0 && nonConstants.Count == 1 && constVal == 1)
                    return nonConstants[0]; // there is only one nonConstant and constant is 1
                else if (constants.Count == 0 && nonConstants.Count > 1 || constants.Count > 0 && nonConstants.Count > 0 && constVal == 1)
                    return new MultiplyOperation { Expressions = nonConstants, Value = Value }; // there are many nonConstants but const multiplier is 1
                else // multiple nonConstants and const <> 1
                    return new MultiplyOperation { Expressions = nonConstants.Concat(new Expression[] { new Constant { Value = constVal } }).ToList(), Value = Value };
            }
        }
    }
    public override Expression DeepClone()
    {
        return new MultiplyOperation { Expressions = Expressions.Select(e => e.DeepClone()).ToList(), Value = Value };
    }

    public override Expression DerivativeOver(Multiplier multiplierExpression)
    {
        if (Expressions.Count == 1)
            return Expressions[0].DerivativeOver(multiplierExpression);
        if (Expressions.Count < 1)
            throw new Exception("Nothing to multiply");

        // derivative of the product of two functions
        SumOperation sumOperation = new SumOperation();
        sumOperation.Expressions.Add(new MultiplyOperation { Expressions = new List<Expression> { Expressions.First().DerivativeOver(multiplierExpression), new MultiplyOperation { Expressions = Expressions.Skip(1).ToList() }.DeepClone() } });
        sumOperation.Expressions.Add(new MultiplyOperation { Expressions = new List<Expression> { Expressions.First().DeepClone(), new MultiplyOperation { Expressions = Expressions.Skip(1).ToList() }.DerivativeOver(multiplierExpression) } });

        var result = sumOperation.Optimised();
        return result;
    }
    public override string ToString()
    {
        return $"({string.Join("*", Expressions.Select(e => e.ToString()))})";
    }
}
public class SumOperation : Expression
{
    public override List<Expression> Expressions { get; set; } = new List<Expression>();
    public override double Value { get; set; }
    public override void Calc()
    {
        Expressions.ForEach(e => e.Calc());
        Value = Expressions.Sum(e => e.Value);
    }

    public override Expression Optimised()
    {
        if (Expressions.All(expr => expr is Constant && (expr as Constant).Value == 0))
        { // if all are constants = 0
            return new Constant() { Value = 0 };
        }
        else
        {
            List<Expression> optimisedExpressions = Expressions.Select(expr => expr.Optimised()).ToList();
            if (optimisedExpressions.All(expr => expr is Constant && (expr as Constant).Value == 0))
            { // if all are const 0 for optimised
                return new Constant() { Value = 0 };
            }
            else
            {
                List<Constant> constants = optimisedExpressions.Where(e => e is Constant).Cast<Constant>().ToList();
                double constVal = constants.Sum(c => c.Value);
                List<Expression> nonConstants = optimisedExpressions.Where(e => !constants.Contains(e)).ToList();
                if (constants.Count > 0 && nonConstants.Count == 0)
                    return new Constant { Value = constVal }; // all const with sum 0
                else if (constants.Count == 0 && nonConstants.Count == 1 || constants.Count > 0 && nonConstants.Count == 1 && constVal == 0)
                    return nonConstants[0]; // all const sum to 0 - there  is one nonConst
                else if (constants.Count == 0 && nonConstants.Count > 1 || constants.Count > 0 && nonConstants.Count > 0 && constVal == 0)
                    return new SumOperation { Expressions = nonConstants, Value = Value }; // all const sum to 0 - there are many nonconst
                else // there many nonconst and many const not summing to 0
                    return new SumOperation { Expressions = nonConstants.Concat(new Expression[] { new Constant { Value = constVal } }).ToList(), Value = Value };
            }
        }
    }
    public override Expression DeepClone()
    {
        return new SumOperation { Expressions = Expressions.Select(e => e.DeepClone()).ToList(), Value = Value };
    }

    public override Expression DerivativeOver(Multiplier multiplierExpression)
    {
        Expression result = new SumOperation { Expressions = Expressions.Select(e => e.DerivativeOver(multiplierExpression)).ToList() }.Optimised();
        return result;
    }
    public override string ToString()
    {
        return $"({string.Join("+", Expressions.Select(e => e.ToString()))})";
    }
}
public class ActivationFunctionOperation : Expression
{
    public ActivationFunction ActivationFunction { get; set; }
    public Expression Expression { get; set; }
    public override List<Expression> Expressions { get { return new List<Expression> { Expression }; } }
    public override double Value { get; set; }
    public override void Calc()
    {
        Expression.Calc();
        Value = ActivationFunction.Apply(Expression.Value);
    }

    public override Expression Optimised()
    {
        if (Expression is Constant && (Expression as Constant).Value == 0)
        {
            return new Constant() { Value = 0 };
        }
        else
        {
            Expression optimisedExpression = Expression.Optimised();
            if (optimisedExpression is Constant && (optimisedExpression as Constant).Value == 0)
            {
                return new Constant() { Value = 0 };
            }
            else
            {
                return new ActivationFunctionOperation { Expression = optimisedExpression, ActivationFunction = ActivationFunction, Value = Value };
            }
        }
    }
    public override Expression DeepClone()
    {
        return new ActivationFunctionOperation { Expression = Expression.DeepClone(), Value = Value, ActivationFunction = ActivationFunction };
    }

    public override Expression DerivativeOver(Multiplier multiplierExpression)
    {
        // Chain rule
        MultiplyOperation result = new MultiplyOperation()
        {
            Expressions = new List<Expression>()
            {
                new ActivationDerivativeFunctionOperation {Expression = new Constant() {Value = Expression.Value}, Value = ActivationFunction.ApplyDerivative(Expression.Value), ActivationFunction = ActivationFunction},
                Expression.DerivativeOver(multiplierExpression).Optimised()
            }
        };
        return result;

    }
    public override string ToString()
    {
        return $"{ActivationFunction.ToLetterCode()}({Expression.ToString()})";
    }
}
public class ActivationDerivativeFunctionOperation : Expression
{
    public ActivationFunction ActivationFunction { get; set; }
    public override double Value { get; set; }
    public Expression Expression { get; set; }
    public override List<Expression> Expressions { get { return new List<Expression> { Expression }; } }
    public override void Calc()
    {
        Expression.Calc();
        Value = ActivationFunction.ApplyDerivative(Expression.Value);
    }
    public override Expression DeepClone()
    {
        return new ActivationDerivativeFunctionOperation { Expression = Expression.DeepClone(), Value = Value, ActivationFunction = ActivationFunction };
    }
    public override Expression DerivativeOver(Multiplier multiplierExpression)
    {
        throw new Exception("Trying to calculate second derivative of the activation function - unexpected");
    }
    public override Expression Optimised()
    {
        if (Expression is Constant)
            return new Constant() { Value = ActivationFunction.ApplyDerivative((Expression as Constant)!.Value) };
        else
        {
            Expression optimisedExpression = Expression.Optimised();

            if (optimisedExpression is Constant)
                return new Constant() { Value = ActivationFunction.ApplyDerivative((Expression as Constant)!.Value) };
            else
            {
                return new ActivationDerivativeFunctionOperation { Expression = optimisedExpression, ActivationFunction = ActivationFunction, Value = Value };
            }
        }
    }

    public override string ToString()
    {
        return $"{ActivationFunction.ToLetterCode()}'({Expression.ToString()})";
    }
}
