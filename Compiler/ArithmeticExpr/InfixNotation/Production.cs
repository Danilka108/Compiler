using CodeAnalysis;

namespace Compiler.ArithmeticExpr.InfixNotation;

internal class ExprProduction
{
    public Span Span { get; init; }

    public TermProduction Term { get; init; }

    public ExprRemProduction Rem { get; init; }

    public ParseResult IntoExpr()
    {
        return Term.IntoExpr().FlatMap(Rem.IntoExpr);
    }
}

internal abstract class ExprRemProduction
{
    public Span Span { get; init; }

    private ExprRemProduction()
    {
    }

    public abstract ParseResult IntoExpr(Expr? lhs);

    public sealed class Sum : ExprRemProduction
    {
        public TermProduction Term { get; init; }

        public ExprRemProduction Rem { get; init; }

        public override ParseResult IntoExpr(Expr? lhs)
        {
            var a = Term.IntoExpr();

            return new ParseResult(lhs)
                .Combine(a, (lhs, rhs) => new OpExpr
                {
                    Span = Span, Lhs = lhs, Rhs = rhs, Kind = OperatorKind.Sum
                })
                .FlatMap(Rem.IntoExpr);
        }
    }

    public sealed class Sub : ExprRemProduction
    {
        public TermProduction Term { get; init; }

        public ExprRemProduction Rem { get; init; }


        public override ParseResult IntoExpr(Expr? lhs)
        {
            return new ParseResult(lhs)
                .Combine(Term.IntoExpr(), (lhs, rhs) => new OpExpr
                {
                    Span = Span, Lhs = lhs, Rhs = rhs, Kind = OperatorKind.Sub
                })
                .FlatMap(Rem.IntoExpr);
        }
    }

    public sealed class Epsilon : ExprRemProduction
    {
        public override ParseResult IntoExpr(Expr lhs)
        {
            return new ParseResult(lhs);
        }
    }

    public sealed class ExpectedOperatorError : ExprRemProduction
    {
        public TermProduction Term { get; init; }

        public ExprRemProduction Rem { get; init; }

        public override ParseResult IntoExpr(Expr? lhs)
        {
            var errors = new ParseResult(new ParseError.ExpectedOperator { Span = Span });

            errors
                .ConsumeErrors(Term.IntoExpr())
                .ConsumeErrors(Rem.IntoExpr(null));

            return errors;
        }
    }
}

internal class TermProduction
{
    public Span Span { get; init; }

    public FactorProduction Factor { get; init; }

    public TermRemProduction Rem { get; init; }

    public ParseResult IntoExpr()
    {
        return Factor.IntoExpr().FlatMap(Rem.IntoExpr);
    }
}

internal abstract class TermRemProduction
{
    public Span Span { get; init; }

    private TermRemProduction()
    {
    }

    public abstract ParseResult IntoExpr(Expr? lhs);

    public sealed class Mul : TermRemProduction
    {
        public FactorProduction Factor { get; init; }

        public TermRemProduction Rem { get; init; }

        public override ParseResult IntoExpr(Expr? lhs)
        {
            return new ParseResult(lhs)
                .Combine(Factor.IntoExpr(), (lhs, rhs) => new OpExpr
                {
                    Span = Span, Lhs = lhs, Rhs = rhs, Kind = OperatorKind.Mul
                })
                .FlatMap(Rem.IntoExpr);
        }
    }

    public sealed class Div : TermRemProduction
    {
        public FactorProduction Factor { get; init; }

        public TermRemProduction Rem { get; init; }

        public override ParseResult IntoExpr(Expr? lhs)
        {
            return new ParseResult(lhs)
                .Combine(Factor.IntoExpr(), (lhs, rhs) => new OpExpr
                {
                    Span = Span, Lhs = lhs, Rhs = rhs, Kind = OperatorKind.Div
                })
                .FlatMap(Rem.IntoExpr);
        }
    }

    public sealed class Epsilon : TermRemProduction
    {
        public override ParseResult IntoExpr(Expr? lhs)
        {
            return new ParseResult(lhs);
        }
    }
}

internal abstract class FactorProduction
{
    public Span Span { get; init; }

    private FactorProduction()
    {
    }

    public abstract ParseResult IntoExpr();

    public sealed class Number : FactorProduction
    {
        public override ParseResult IntoExpr()
        {
            return new ParseResult(new ArithmeticExpr.Number { Span = Span });
        }
    }

    public sealed class ExpectedOperand : FactorProduction
    {
        public override ParseResult IntoExpr()
        {
            return new ParseResult(new ParseError.ExpectedOperand { Span = Span });
        }
    }

    public sealed class Delimited : FactorProduction
    {
        public ExprProduction Expr { get; init; }

        public override ParseResult IntoExpr()
        {
            return Expr.IntoExpr();
        }
    }

    public sealed class ExpectedCloseBracketError : FactorProduction
    {
        public ExprProduction Expr { get; init; }

        public override ParseResult IntoExpr()
        {
            return Expr.IntoExpr().AddError(new ParseError.ExpectedCloseBracket { Span = Span });
        }
    }

    public sealed class ExpectedOpenBracketError : FactorProduction
    {
        public ExprProduction Expr { get; init; }

        public override ParseResult IntoExpr()
        {
            return Expr.IntoExpr().AddError(new ParseError.ExpectedOpenBracket { Span = Span });
        }
    }

    public sealed class UnexpectedSymbol : FactorProduction
    {
        public override ParseResult IntoExpr()
        {
            return new ParseResult(new ParseError.UnexpectedSymbol { Span = Span });
        }
    }
}