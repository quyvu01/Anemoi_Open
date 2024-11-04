using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace Anemoi.BuildingBlock.Application.Helpers;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public sealed class ExpressionHelper
{
    public static Expression<Func<T, TProp>> CombineAnd<T, TProp>(
        params Expression<Func<T, TProp>>[] expressionFunctions)
        => expressionFunctions.Aggregate(AndAlso);

    public static Expression<Func<T, TProp>> CombineOr<T, TProp>(
        params Expression<Func<T, TProp>>[] expressionFunctions)
        => expressionFunctions.Aggregate(OrElse);

    public static Expression<Func<T, TProp>> Combine<T, TProp>(Expression<Func<T, TProp>> expressionFunction)
        => expressionFunction;

    public static Expression<Func<T, TProp>> AndAlso<T, TProp>(Expression<Func<T, TProp>> expr1,
        Expression<Func<T, TProp>> expr2)
    {
        switch (expr1)
        {
            case null when expr2 is null:
                throw new ArgumentNullException($"Expression: {nameof(expr1)} and {nameof(expr2)} cannot be null!");
            case null:
                return expr2;
        }

        if (expr2 is null) return expr1;

        var parameter = Expression.Parameter(typeof(T));

        var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
        var left = leftVisitor.Visit(expr1.Body);

        var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
        var right = rightVisitor.Visit(expr2.Body);

        return Expression.Lambda<Func<T, TProp>>(Expression.AndAlso(left!, right!), parameter);
    }

    public static Expression<Func<T, TProp>> OrElse<T, TProp>(Expression<Func<T, TProp>> expr1,
        Expression<Func<T, TProp>> expr2)
    {
        switch (expr1)
        {
            case null when expr2 is null:
                throw new ArgumentNullException($"Expression: {nameof(expr1)} and {nameof(expr2)} cannot be null!");
            case null:
                return expr2;
        }

        if (expr2 is null) return expr1;

        var parameter = Expression.Parameter(typeof(T));

        var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
        var left = leftVisitor.Visit(expr1.Body);

        var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
        var right = rightVisitor.Visit(expr2.Body);

        return Expression.Lambda<Func<T, TProp>>(Expression.OrElse(left!, right!), parameter);
    }

    private class ReplaceExpressionVisitor(Expression oldValue, Expression newValue) : ExpressionVisitor
    {
        public override Expression Visit(Expression node) => node == oldValue ? newValue : base.Visit(node);
    }
}