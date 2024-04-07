using System;
using Compiler.Parser;
using Scanner;

namespace Compiler.ViewModels;

public class EditorTokenViewModel(CaretPos caretPos, LexemeType lexemeType, string content) : ViewModelBase
{
    // public Token<TokenType, TokenError> Token { get; } = token;
    public LexemeType LexemeType { get; } = lexemeType;

    public CaretPos CaretPos { get; } = caretPos;

    // public string Code => Lexeme is Token<TokenType, TokenError>.ValidToken t ? ((int)t.Type).ToString() : "";
    public string Code => LexemeType is LexemeType.Valid v ? ((int)v.Type).ToString() : "";

    public string Value => content[LexemeType.Span.Start..LexemeType.Span.End];
}