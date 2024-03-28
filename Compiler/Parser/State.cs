using Scanner;

namespace Compiler.parser;

public enum StateKind
{
    Start,
    Lexeme,
    End
}

public enum ParseErrorType
{
    ConstKeywordExpected = 1,
    IdentifierExpected = 2,
    TypeDividerExpected = 3,
    LinkExpected = 4,
    StrTypeExpected = 5,
    AssignmentOperatorExpected = 6,
    StringLiteralExpected = 7,
    OperatorEndExpected = 8,
    NothingExpected = 9,
    SeparatorExpected = 10
}

public struct ParseError
{
    public ParseErrorType Type { get; }

    public Span Span { get; }

    public ParseError(ParseErrorType type, Span span)
    {
        Type = type;
        Span = span;
    }
}

internal class State
{
    public StateKind StateKind { get; set; }
    public LexemeType Lexeme { get; set; }
}

public class Parser
{
    private Parser()
    {
    }

    public static ParseError? Parse(string content)
    {
        var state = new State();
        var lexemes = Lexer.Scan(content);

        foreach (var lexeme in lexemes)
            if (lexeme is Lexeme.Valid validLexeme && OnNext(state, validLexeme) is { } errorType)
                return new ParseError(errorType, lexeme.Span);

        if (OnNext(state, null) is { } lastErrorType)
            return new ParseError(lastErrorType, new Span(content.Length, content.Length));

        return null;
    }

    private static ParseErrorType? OnNext(State state, Lexeme.Valid? lexeme)
    {
        switch (state)
        {
            // Start
            case { StateKind: StateKind.Start } when lexeme?.Type == LexemeType.ConstKeyword:
                state.StateKind = StateKind.Lexeme;
                state.Lexeme = LexemeType.ConstKeyword;
                return null;
            case { StateKind: StateKind.Start }:
                return ParseErrorType.ConstKeywordExpected;
            // Const keyword
            case { StateKind: StateKind.Lexeme, Lexeme: LexemeType.ConstKeyword } when
                lexeme?.Type == LexemeType.Identifier:
                state.Lexeme = LexemeType.Identifier;
                return null;
            case { StateKind: StateKind.Lexeme, Lexeme: LexemeType.ConstKeyword }:
                return ParseErrorType.IdentifierExpected;
            // Identifier
            case { StateKind: StateKind.Lexeme, Lexeme: LexemeType.Identifier } when
                lexeme?.Type == LexemeType.Colon:
                state.Lexeme = LexemeType.Colon;
                return null;
            case { StateKind: StateKind.Lexeme, Lexeme: LexemeType.Identifier }:
                return ParseErrorType.TypeDividerExpected;
            // Colon
            case { StateKind: StateKind.Lexeme, Lexeme: LexemeType.Colon } when
                lexeme?.Type == LexemeType.Ampersand:
                state.Lexeme = LexemeType.Ampersand;
                return null;
            case { StateKind: StateKind.Lexeme, Lexeme: LexemeType.Colon }:
                return ParseErrorType.LinkExpected;
            // Ampersand
            case { StateKind: StateKind.Lexeme, Lexeme: LexemeType.Ampersand } when
                lexeme?.Type == LexemeType.StrKeyword:
                state.Lexeme = LexemeType.StrKeyword;
                return null;
            case { StateKind: StateKind.Lexeme, Lexeme: LexemeType.Ampersand }:
                return ParseErrorType.StrTypeExpected;
            // Str keyword
            case { StateKind: StateKind.Lexeme, Lexeme: LexemeType.StrKeyword } when
                lexeme?.Type == LexemeType.AssignmentOperator:
                state.Lexeme = LexemeType.AssignmentOperator;
                return null;
            case { StateKind: StateKind.Lexeme, Lexeme: LexemeType.StrKeyword }:
                return ParseErrorType.AssignmentOperatorExpected;
            // Assignment operator
            case { StateKind: StateKind.Lexeme, Lexeme: LexemeType.AssignmentOperator } when
                lexeme?.Type == LexemeType.StringLiteral:
                state.Lexeme = LexemeType.StringLiteral;
                return null;
            case { StateKind: StateKind.Lexeme, Lexeme: LexemeType.AssignmentOperator }:
                return ParseErrorType.StringLiteralExpected;
            // String literal
            case { StateKind: StateKind.Lexeme, Lexeme: LexemeType.StringLiteral } when
                lexeme?.Type == LexemeType.OperatorEnd:
                state.Lexeme = LexemeType.OperatorEnd;
                return null;
            case { StateKind: StateKind.Lexeme, Lexeme: LexemeType.StringLiteral }:
                return ParseErrorType.OperatorEndExpected;
            // End
            case { StateKind: StateKind.End } when lexeme == null:
                return null;
            case { StateKind: StateKind.End }:
                return ParseErrorType.NothingExpected;
            default:
                return null;
        }
    }
}