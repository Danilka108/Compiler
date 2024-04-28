using System;
using CodeAnalysis;

namespace Compiler.ArithmeticExpr;

public abstract class Expr
{
    public Span Span { get; init; }

    public abstract string IntoPostfixNotation(string source);

    public abstract long? Calculate(string source);
}

class Number : Expr
{
    public override string IntoPostfixNotation(string source)
    {
        return source.Substring(Span.Start, Span.Count);
    }

    public override long? Calculate(string source)
    {
        var substring = source.Substring(Span.Start, Span.Count);
        return long.TryParse(substring, out var value) ? value : null;
    }
}

class OpExpr : Expr
{
    public OperatorKind Kind { get; init; }

    public Expr Lhs { get; init; }

    public Expr Rhs { get; init; }

    public override string IntoPostfixNotation(string source)
    {
        return $"{Kind.IntoString()} {Lhs.IntoPostfixNotation(source)} {Rhs.IntoPostfixNotation(source)}";
    }

    public override long? Calculate(string source)
    {
        var lhs = Lhs.Calculate(source);
        var rhs = Rhs.Calculate(source);

        return Kind switch
        {
            OperatorKind.Sum => lhs + rhs,
            OperatorKind.Sub => lhs - rhs,
            OperatorKind.Mul => lhs * rhs,
            OperatorKind.Div when rhs == 0 => null,
            OperatorKind.Div => lhs / rhs,
        };
    }
}

public enum OperatorKind
{
    Sum,
    Sub,
    Mul,
    Div,
}

public static class OpKindExtensions
{
    public static string IntoString(this OperatorKind operatorKind)
    {
        return operatorKind switch
        {
            OperatorKind.Sum => "+",
            OperatorKind.Sub => "-",
            OperatorKind.Mul => "*",
            OperatorKind.Div => "/",
        };
    }

    public static int Priority(this OperatorKind operatorKind)
    {
        return operatorKind switch
        {
            OperatorKind.Sum => 1,
            OperatorKind.Sub => 1,
            OperatorKind.Mul => 2,
            OperatorKind.Div => 2,
        };
    }

    public static OperatorKind IntoOperatorKind(this LexemeType lexemeType)
    {
        return lexemeType switch
        {
            LexemeType.Plus => OperatorKind.Sum,
            LexemeType.Dash => OperatorKind.Sub,
            LexemeType.Asterisk => OperatorKind.Mul,
            LexemeType.Slash => OperatorKind.Div,
            _ => throw new ArgumentOutOfRangeException(nameof(lexemeType), lexemeType, null)
        };
    }
}