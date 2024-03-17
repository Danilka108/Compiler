using System.Collections;
using System.Collections.Generic;
using Optional;
using Scanner;

namespace Compiler;

using Token = Token<TokenType, TokenError>;

public enum State
{
    Start,
    Token,
    End
}

public enum ParseErrorType
{
    ConstKeywordExpected,
    IdentifierExpected,
    TypeDividerExpected,
    LinkExpected,
    StrTypeExpected,
    AssignmentOperatorExpected,
    StringLiteralExpected,
    OperatorEndExpected,
    NothingExpected
}

public struct ParseError
{
    public ParseErrorType Type { get; internal init; }

    public Span Span { get; internal init; }
}

public class Parser : IEnumerable<ParseError>
{
    private readonly IEnumerable<Token> _tokens;

    private State _state;
    private TokenType _tokenType;
    private Option<ParseErrorType> _error;

    public Parser(IEnumerable<Token> tokens)
    {
        _tokens = tokens;
        _state = State.Start;
        _tokenType = default;
        _error = Option.None<ParseErrorType>();
    }

    public IEnumerator<ParseError> GetEnumerator()
    {
        _state = State.Start;
        _tokenType = default;
        _error = Option.None<ParseErrorType>();

        foreach (var token in _tokens)
        {
            if (token is not Token.ValidToken validToken) continue;
            var nextTokenType = validToken.Type;

            if (nextTokenType is TokenType.Separator) continue;

            _error = Option.None<ParseErrorType>();
            OnNextTokenType(nextTokenType.Some());

            foreach (var error in _error) yield return new ParseError { Type = error, Span = token.Span };
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private void OnNextTokenType(Option<TokenType> nextTokenType)
    {
        if (_state is State.Start)
        {
            _state = State.Token;
            _error = nextTokenType.Contains(TokenType.ConstKeyword)
                ? Option.None<ParseErrorType>()
                : ParseErrorType.ConstKeywordExpected.Some();
            _tokenType = _error.HasValue ? _tokenType : TokenType.ConstKeyword;

            return;
        }

        if (_state is State.Token && _tokenType is TokenType.ConstKeyword)
        {
            _error = nextTokenType.Contains(TokenType.Identifier)
                ? Option.None<ParseErrorType>()
                : ParseErrorType.IdentifierExpected.Some();
            _tokenType = _error.HasValue ? _tokenType : TokenType.Identifier;

            return;
        }

        if (_state is State.Token && _tokenType is TokenType.Identifier)
        {
            _error = nextTokenType.Contains(TokenType.Colon)
                ? Option.None<ParseErrorType>()
                : ParseErrorType.TypeDividerExpected.Some();
            _tokenType = _error.HasValue ? _tokenType : TokenType.Colon;

            return;
        }

        if (_state is State.Token && _tokenType is TokenType.Colon)
        {
            _error = nextTokenType.Contains(TokenType.Ampersand)
                ? Option.None<ParseErrorType>()
                : ParseErrorType.LinkExpected.Some();
            _tokenType = _error.HasValue ? _tokenType : TokenType.Ampersand;

            return;
        }

        if (_state is State.Token && _tokenType is TokenType.Ampersand)
        {
            _error = nextTokenType.Contains(TokenType.StrKeyword)
                ? Option.None<ParseErrorType>()
                : ParseErrorType.StrTypeExpected.Some();
            _tokenType = _error.HasValue ? _tokenType : TokenType.StrKeyword;

            return;
        }

        if (_state is State.Token && _tokenType is TokenType.StrKeyword)
        {
            _error = nextTokenType.Contains(TokenType.AssignmentOperator)
                ? Option.None<ParseErrorType>()
                : ParseErrorType.AssignmentOperatorExpected.Some();
            _tokenType = _error.HasValue ? _tokenType : TokenType.AssignmentOperator;

            return;
        }

        if (_state is State.Token && _tokenType is TokenType.AssignmentOperator)
        {
            _error = nextTokenType.Contains(TokenType.StringLiteral)
                ? Option.None<ParseErrorType>()
                : ParseErrorType.StringLiteralExpected.Some();
            _tokenType = _error.HasValue ? _tokenType : TokenType.StringLiteral;

            return;
        }

        if (_state is State.Token && _tokenType is TokenType.StringLiteral)
        {
            _error = nextTokenType.Contains(TokenType.OperatorEnd)
                ? Option.None<ParseErrorType>()
                : ParseErrorType.OperatorEndExpected.Some();
            _tokenType = _error.HasValue ? _tokenType : TokenType.OperatorEnd;

            return;
        }

        if (_state is State.Token && _tokenType is TokenType.OperatorEnd)
        {
            _state = State.End;
            _error = nextTokenType.HasValue
                ? ParseErrorType.NothingExpected.Some()
                : Option.None<ParseErrorType>();

            return;
        }

        if (_state is State.End)
        {
            _error = nextTokenType.HasValue
                ? ParseErrorType.NothingExpected.Some()
                : Option.None<ParseErrorType>();
            return;
        }
    }
}