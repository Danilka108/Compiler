using System;
using System.Collections.Generic;
using System.Linq;
using CodeAnalysis;

namespace Compiler.ArithmeticExpr.InfixNotation;

internal class Parser
{
    private Lexeme<LexemeType>[] _lexemes = Array.Empty<Lexeme<LexemeType>>();
    private int _cursor = 0;
    private readonly Stack<Span> _unclosedDelimiters = [];

    private ExprProduction? _expr = null;
    private Span? _unopenedErrorSpan = null;


    public ParseResult Parse(IEnumerable<Lexeme<LexemeType>> lexemes)
    {
        _lexemes = lexemes.Where(lexeme => lexeme.Type != LexemeType.Separator).ToArray();
        _unclosedDelimiters.Clear();
        _cursor = 0;


        while (!IsEnd())
        {
            _expr = ParseExpr();

            if (Check(LexemeType.CloseBracket))
            {
                _unopenedErrorSpan = Bump();
            }
        }

        if (_unopenedErrorSpan != null)
        {
            _expr = ParseExpr();
        }

        return _expr.IntoExpr();
    }

    private ExprProduction ParseExpr()
    {
        var startSpan = Span();

        var term = ParseTerm();
        var rem = ParseExprRem();

        return new ExprProduction { Term = term, Rem = rem, Span = new Span(startSpan.Start, Span().Start) };
    }

    private ExprRemProduction ParseExprRem()
    {
        if (TryBump(LexemeType.Plus) is { } plusSpan)
        {
            var term = ParseTerm();
            var rem = ParseExprRem();
            return new ExprRemProduction.Sum { Term = term, Rem = rem, Span = plusSpan };
        }

        if (TryBump(LexemeType.Dash) is { } dashSpan)
        {
            var term = ParseTerm();
            var rem = ParseExprRem();
            return new ExprRemProduction.Sub { Term = term, Rem = rem, Span = dashSpan };
        }

        if (IsEnd() || Check(LexemeType.CloseBracket))
        {
            return new ExprRemProduction.Epsilon { Span = Span() };
        }

        return new ExprRemProduction.ExpectedOperatorError
        {
            Span = Span(),
            Term = ParseTerm(),
            Rem = ParseExprRem(),
        };
    }

    private TermProduction ParseTerm()
    {
        var startSpan = Span();
        FactorProduction factor;

        if (_unopenedErrorSpan != null)
        {
            factor = new FactorProduction.ExpectedOpenBracketError
            {
                Span = _unopenedErrorSpan.Value,
                Expr = _expr,
            };
            _unopenedErrorSpan = null;
        }
        else
        {
            factor = ParseFactor();
        }

        var rem = ParseTermRem();

        return new TermProduction { Factor = factor, Rem = rem, Span = new Span(startSpan.Start, Span().Start) };
    }

    private TermRemProduction ParseTermRem()
    {
        if (TryBump(LexemeType.Asterisk) is { } asteriskSpan)
        {
            var factor = ParseFactor();
            var rem = ParseTermRem();
            return new TermRemProduction.Mul { Factor = factor, Rem = rem, Span = asteriskSpan };
        }

        if (TryBump(LexemeType.Slash) is { } slashSpan)
        {
            var factor = ParseFactor();
            var rem = ParseTermRem();
            return new TermRemProduction.Div { Factor = factor, Rem = rem, Span = slashSpan };
        }

        return new TermRemProduction.Epsilon { Span = Span() };
    }

    private FactorProduction ParseFactor()
    {
        if (TryBump(LexemeType.UnexpectedSymbol) is { } unexpectedSymbolSpan)
        {
            return new FactorProduction.UnexpectedSymbol
            {
                Span = unexpectedSymbolSpan,
            };
        }

        if (TryBump(LexemeType.Number) is { } numberSpan)
        {
            return new FactorProduction.Number { Span = numberSpan };
        }

        if (TryBump(LexemeType.OpenBracket) is { } openSpan)
        {
            var expr = ParseExpr();

            _unclosedDelimiters.Push(openSpan);

            if (TryBump(LexemeType.CloseBracket) is { } closeSpan)
            {
                _unclosedDelimiters.Pop();

                var endSpan = Span();
                return new FactorProduction.Delimited
                {
                    Span = new Span(openSpan.Start, closeSpan.End),
                    Expr = expr,
                };
            }

            return new FactorProduction.ExpectedCloseBracketError
            {
                Span = openSpan,
                Expr = expr,
            };
        }

        return new FactorProduction.ExpectedOperand { Span = Span() };
    }

    private Span Span()
    {
        return _cursor < _lexemes.Length ? _lexemes[_cursor].Span : _lexemes.Last().Span.ShiftStartToEnd();
    }

    private Span Bump()
    {
        var span = Span();
        _cursor += 1;
        return span;
    }

    private bool IsEnd()
    {
        return _cursor >= _lexemes.Length;
    }

    private Span? TryBump(LexemeType lexemeType)
    {
        if (_cursor < _lexemes.Length && _lexemes[_cursor].Type == lexemeType)
        {
            var span = Span();
            _cursor += 1;
            return span;
        }

        return null;
    }

    private bool Check(LexemeType lexemeType)
    {
        return _cursor < _lexemes.Length && _lexemes[_cursor].Type == lexemeType;
    }
}