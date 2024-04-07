using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.Parser;

public enum StateKind
{
    Start,
    Lexeme,
    End
}

public enum ParseErrorType
{
    UnexpectedSymbol = 1,
    UnterminatedString,
    ConstKeywordExpected,
    IdentifierExpected,
    TypeDividerExpected,
    LinkExpected,
    StrTypeExpected,
    AssignmentOperatorExpected,
    StringLiteralExpected,
    OperatorEndExpected,
    NothingExpected,
    SeparatorExpected
}

public readonly struct ParseError(ParseErrorType type, Span span)
{
    public ParseErrorType Type { get; } = type;

    public Span Span { get; } = span;
}

internal class State
{
    private StateKind StateKind { get; set; } = StateKind.Start;

    private LexemeType Lexeme { get; set; } = default;

    public bool IsLexeme(LexemeType type)
    {
        return StateKind == StateKind.Lexeme && Lexeme == type;
    }

    public void SwitchToLexeme(LexemeType type)
    {
        StateKind = StateKind.Lexeme;
        Lexeme = type;
    }

    public void SwitchToEnd()
    {
        StateKind = StateKind.End;
    }

    public bool IsStart()
    {
        return StateKind == StateKind.Start;
    }

    public bool IsEnd()
    {
        return StateKind == StateKind.End;
    }
}

public class Parser
{
    public static IEnumerable<ParseError> Scan(string content)
    {
        var lexemes = Lexer.Scan(content);
        var parser = new Parser(lexemes, content.Length);

        return parser.Parse();
    }

    private State State { get; set; }

    private Lexeme[] Lexemes { get; }

    private int ContentLength { get; }

    private Parser(IEnumerable<Lexeme> lexemes, int contentLength)
    {
        State = new State();
        Lexemes = lexemes.ToArray();
        ContentLength = contentLength;
    }

    private IEnumerable<ParseError> Parse()
    {
        var lastSpan = new Span();
        var wasError = false;

        foreach (var lexeme in Lexemes)
        {
            lastSpan = lexeme.Span;

            // if (wasError && lexeme is Lexeme.Valid validLexeme)
            // {
            //     State.SwitchToLexeme(validLexeme.Type);
            // }

            wasError = false;
            if (HandleLexeme(lexeme.Span, lexeme) is { } parseError)
            {
                yield return parseError;
                wasError = true;
            }
        }

        lastSpan = new Span(int.Min(lastSpan.End, ContentLength - 1), ContentLength);
        if (HandleLexeme(lastSpan, null) is { } lastParseError)
        {
            yield return lastParseError;
        }
    }

    private ParseError? HandleLexeme(Span span, Lexeme? nextLexeme)
    {
        if (Switch(nextLexeme) is not { } errorType) return null;
        var error = new ParseError(errorType, span);

        switch (error.Type)
        {
            case (ParseErrorType.UnexpectedSymbol):
                break;
            case (ParseErrorType.UnterminatedString):
                break;
            case (ParseErrorType.ConstKeywordExpected):
                State.SwitchToLexeme(LexemeType.ConstKeyword);
                break;
            case (ParseErrorType.IdentifierExpected):
                State.SwitchToLexeme(LexemeType.Identifier);
                break;
            case (ParseErrorType.TypeDividerExpected):
                State.SwitchToLexeme(LexemeType.Colon);
                break;
            case (ParseErrorType.LinkExpected):
                State.SwitchToLexeme(LexemeType.Ampersand);
                break;
            case (ParseErrorType.StrTypeExpected):
                State.SwitchToLexeme(LexemeType.StrKeyword);
                break;
            case (ParseErrorType.AssignmentOperatorExpected):
                State.SwitchToLexeme(LexemeType.AssignmentOperator);
                break;
            case (ParseErrorType.StringLiteralExpected):
                State.SwitchToLexeme(LexemeType.StringLiteral);
                break;
            case (ParseErrorType.OperatorEndExpected):
                State.SwitchToLexeme(LexemeType.OperatorEnd);
                break;
            case (ParseErrorType.NothingExpected):
                State.SwitchToEnd();
                break;
            case ParseErrorType.SeparatorExpected:
                break;
            // default:
            //     break;
            // throw new ArgumentOutOfRangeException();
        }

        return error;
    }

    private ParseErrorType? Switch(Lexeme? nextLexeme)
    {
        if (nextLexeme is Lexeme.Invalid invalidLexeme)
        {
            return invalidLexeme.Error switch
            {
                InvalidLexemeType.UnexpectedSymbol => ParseErrorType.UnexpectedSymbol,
                InvalidLexemeType.UnterminatedString => ParseErrorType.UnterminatedString,
            };
        }

        var validLexeme = nextLexeme as Lexeme.Valid;
        if (validLexeme?.Type == LexemeType.Separator)
        {
            return null;
        }

        return SwitchToConstKeyword(nextLexeme as Lexeme.Valid);
    }

    private ParseErrorType? SwitchToConstKeyword(Lexeme.Valid? nextLexeme)
    {
        if (State.IsStart() && nextLexeme?.Type == LexemeType.ConstKeyword)
        {
            State.SwitchToLexeme(LexemeType.ConstKeyword);
            return null;
        }

        if (State.IsStart())
        {
            return ParseErrorType.ConstKeywordExpected;
        }

        return SwitchToIdentifier(nextLexeme);
    }

    private ParseErrorType? SwitchToIdentifier(Lexeme.Valid? nextLexeme)
    {
        if (State.IsLexeme(LexemeType.ConstKeyword) && nextLexeme?.Type == LexemeType.Identifier)
        {
            State.SwitchToLexeme(LexemeType.Identifier);
            return null;
        }

        if (State.IsLexeme(LexemeType.ConstKeyword))
        {
            return ParseErrorType.IdentifierExpected;
        }

        return SwitchToTypeDivider(nextLexeme);
    }

    private ParseErrorType? SwitchToTypeDivider(Lexeme.Valid? nextLexeme)
    {
        if (State.IsLexeme(LexemeType.Identifier) && nextLexeme?.Type == LexemeType.Colon)
        {
            State.SwitchToLexeme(LexemeType.Colon);
            return null;
        }

        if (State.IsLexeme(LexemeType.Identifier))
        {
            return ParseErrorType.TypeDividerExpected;
        }

        return SwitchToLinkType(nextLexeme);
    }

    private ParseErrorType? SwitchToLinkType(Lexeme.Valid? nextLexeme)
    {
        if (State.IsLexeme(LexemeType.Colon) && nextLexeme?.Type == LexemeType.Ampersand)
        {
            State.SwitchToLexeme(LexemeType.Ampersand);
            return null;
        }

        if (State.IsLexeme(LexemeType.Colon))
        {
            return ParseErrorType.LinkExpected;
        }

        return SwitchToStrKeyword(nextLexeme);
    }

    private ParseErrorType? SwitchToStrKeyword(Lexeme.Valid? nextLexeme)
    {
        if (State.IsLexeme(LexemeType.Ampersand) && nextLexeme?.Type == LexemeType.StrKeyword)
        {
            State.SwitchToLexeme(LexemeType.StrKeyword);
            return null;
        }

        if (State.IsLexeme(LexemeType.Ampersand))
        {
            return ParseErrorType.StrTypeExpected;
        }

        return SwitchToAssignmentOperator(nextLexeme);
    }

    private ParseErrorType? SwitchToAssignmentOperator(Lexeme.Valid? nextLexeme)
    {
        if (State.IsLexeme(LexemeType.StrKeyword) && nextLexeme?.Type == LexemeType.AssignmentOperator)
        {
            State.SwitchToLexeme(LexemeType.AssignmentOperator);
            return null;
        }

        if (State.IsLexeme(LexemeType.StrKeyword))
        {
            return ParseErrorType.AssignmentOperatorExpected;
        }

        return SwitchToStringLiteral(nextLexeme);
    }

    private ParseErrorType? SwitchToStringLiteral(Lexeme.Valid? nextLexeme)
    {
        if (State.IsLexeme(LexemeType.AssignmentOperator) && nextLexeme?.Type == LexemeType.StringLiteral)
        {
            State.SwitchToLexeme(LexemeType.StringLiteral);
            return null;
        }

        if (State.IsLexeme(LexemeType.AssignmentOperator))
        {
            return ParseErrorType.StringLiteralExpected;
        }

        return SwitchToOperatorEnd(nextLexeme);
    }

    private ParseErrorType? SwitchToOperatorEnd(Lexeme.Valid? nextLexeme)
    {
        if (State.IsLexeme(LexemeType.StringLiteral) && nextLexeme?.Type == LexemeType.OperatorEnd)
        {
            State.SwitchToLexeme(LexemeType.OperatorEnd);
            return null;
        }

        if (State.IsLexeme(LexemeType.StringLiteral))
        {
            return ParseErrorType.OperatorEndExpected;
        }

        return SwitchToEnd(nextLexeme);
    }

    private ParseErrorType? SwitchToEnd(Lexeme.Valid? nextLexeme)
    {
        if (State.IsLexeme(LexemeType.OperatorEnd) && nextLexeme is null)
        {
            State.SwitchToEnd();
            return null;
        }

        if (State.IsLexeme(LexemeType.OperatorEnd))
        {
            return ParseErrorType.NothingExpected;
        }

        return null;
    }
}