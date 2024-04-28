using CodeAnalysis;
using Compiler.ArithmeticExpr;

namespace Compiler.ViewModels;

public class EditorTokenViewModel(ITextEditor editor, Lexeme<LexemeType> lexeme)
    : ViewModelBase
{
    public Lexeme<LexemeType> Lexeme { get; } = lexeme;

    public CaretPos CaretPos { get; } = editor.OffsetToCaretPos(lexeme.Span.Start);

    // public string Value { get; } = editor.Document.Text[lexeme.Span.Start..lexeme.Span.End];
    public string Value { get; } = editor.Document.Text.Substring(lexeme.Span.Start, lexeme.Span.Count);
}