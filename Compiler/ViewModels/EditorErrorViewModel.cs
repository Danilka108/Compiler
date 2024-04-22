using CodeAnalysis;
using Compiler.ConstExpr;

namespace Compiler.ViewModels;

public class EditorErrorViewModel(ITextEditor editor, AnalyzerError<LexemeType> error)
    : ViewModelBase
{
    public CaretPos CaretPos { get; } = editor.OffsetToCaretPos(error.Span.Start);

    public AnalyzerError<LexemeType> Error { get; } = error;

    public LexemeType? LexemeType { get; } = error switch
    {
        AnalyzerError<LexemeType>.LexemesExhausted e => e.ExpectedLexemeType,
        AnalyzerError<LexemeType>.LexemeNotFound e => e.ExpectedLexemeType,
        AnalyzerError<LexemeType>.InvalidLexeme e => null,
        AnalyzerError<LexemeType>.LexemeNotFoundImmediately e => e.ExpectedLexemeType,
    };

    public string Found { get; } = editor.Document.Text.Substring(error.Span.Start, error.Span.Count);

    public string Tail { get; } = editor.Document.Text[error.TailStart..];
}