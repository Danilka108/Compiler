using System;
using Compiler.Parser;
using Scanner;

namespace Compiler.ViewModels;

public class EditorTokenViewModel(CaretPos caretPos, Lexeme lexeme, string content) : ViewModelBase
{
    // public Token<TokenType, TokenError> Token { get; } = token;
    public Lexeme Lexeme { get; } = lexeme;

    public CaretPos CaretPos { get; } = caretPos;

    // public string Code => Lexeme is Token<TokenType, TokenError>.ValidToken t ? ((int)t.Type).ToString() : "";
    public string Code => Lexeme is Lexeme.Valid v ? ((int)v.Type).ToString() : "";

    public string Value => content[Lexeme.Span.Start..Lexeme.Span.End];
}