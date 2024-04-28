using CodeAnalysis;

namespace Compiler.ArithmeticExpr.InfixNotation;

public class ParseError
{
    public Span Span { get; init; }

    public class ExpectedCloseBracket : ParseError;

    public class ExpectedOpenBracket : ParseError;

    public class ExpectedOperator : ParseError;

    public class ExpectedOperand : ParseError;

    public class UnexpectedSymbol : ParseError;
}