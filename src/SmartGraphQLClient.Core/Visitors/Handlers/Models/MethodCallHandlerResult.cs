using System.Linq.Expressions;

namespace SmartGraphQLClient.Core.Visitors.Handlers.Models;

public class MethodCallHandlerResult
{
    public bool CanHandle { get; private set; }
    public MemberExpression MemberExpression { get; private set; } = default!;
    public object? Value { get; private set; }
    public LambdaExpression InternalExpression { get; private set; } = default!;
    public Func<bool, object?> ValueFactory { get; private set; } = default!;
    public string Operator { get; private set; } = string.Empty;
    public string? InvertedOperator { get; private set; }
    internal MethodCallHandlerResultType ResultType { get; private set; }

    private MethodCallHandlerResult()
    {
    }
    
    public static MethodCallHandlerResult Success(
        MemberExpression memberExpression,
        object? value,
        string op,
        string? invertedOp = null)
        => new()
        {
            CanHandle = true,
            ResultType = MethodCallHandlerResultType.MemberWithValue,
            MemberExpression = memberExpression,
            Value = value,
            Operator = op,
            InvertedOperator = invertedOp,
        };

    public static MethodCallHandlerResult Success(
        MemberExpression memberExpression,
        LambdaExpression internalExpression,
        string op,
        string? invertedOp = null)
        => new()
        {
            CanHandle = true,
            ResultType = MethodCallHandlerResultType.MemberWithInternalExpression,
            MemberExpression = memberExpression,
            InternalExpression = internalExpression,
            Operator = op,
            InvertedOperator = invertedOp,
        };
    
    public static MethodCallHandlerResult Success(
        MemberExpression memberExpression,
        Func<bool, object?> valueFactory,
        string op,
        string? invertedOp = null)
        => new()
        {
            CanHandle = true,
            ResultType = MethodCallHandlerResultType.MemberWithValueFactory,
            MemberExpression = memberExpression,
            ValueFactory = valueFactory,
            Operator = op,
            InvertedOperator = invertedOp,
        };

    public static MethodCallHandlerResult Failed()
        => new()
        {
            CanHandle = false
        };

    public string GetOperator(bool inverted)
    {
        if (!inverted) return Operator;

        if (InvertedOperator is null)
        {
            throw new ArgumentException("There is no inverted operator");
        }

        return InvertedOperator;
    }
}

internal enum MethodCallHandlerResultType
{
    MemberWithValue,
    MemberWithInternalExpression,
    MemberWithValueFactory
}