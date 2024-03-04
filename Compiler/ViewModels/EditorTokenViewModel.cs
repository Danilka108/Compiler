using System;
using Scanner;

namespace Compiler.ViewModels;

public class EditorTokenViewModel(Token<TokenType, TokenError> token, string content) : ViewModelBase
{
    public Token<TokenType, TokenError> Token { get; } = token;

    public string Code => Token is Token<TokenType, TokenError>.ValidToken t ? ((int)t.Type).ToString() : "";

    public string Value => Token is Token<TokenType, TokenError>.ValidToken validToken
        ? content[validToken.Span.Start..validToken.Span.End]
        : "";
}