using System;
using Scanner;

namespace Compiler.ViewModels;

public class EditorTokenViewModel(CaretPos caretPos, Token<TokenType, TokenError> token, string content) : ViewModelBase
{
    public Token<TokenType, TokenError> Token { get; } = token;

    public CaretPos CaretPos { get; } = caretPos;

    public string Code => Token is Token<TokenType, TokenError>.ValidToken t ? ((int)t.Type).ToString() : "";

    public string Value => content[Token.Span.Start..Token.Span.End];
}