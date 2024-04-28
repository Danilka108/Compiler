using CodeAnalysis;
using Compiler.ArithmeticExpr.InfixNotation;

namespace Compiler.ViewModels;

public class EditorErrorViewModel(ITextEditor editor, ParseError error)
    : ViewModelBase
{
    public CaretPos CaretPos { get; } = editor.OffsetToCaretPos(error.Span.Start);

    public ParseError Error { get; } = error;

    // public LexemeType? LexemeType { get; } = error switch
    // {
    //     AnalyzerError<LexemeType>.LexemesExhausted e => e.ExpectedLexemeType,
    //     AnalyzerError<LexemeType>.LexemeNotFound e => e.ExpectedLexemeType,
    //     AnalyzerError<LexemeType>.InvalidLexeme e => null,
    //     AnalyzerError<LexemeType>.LexemeNotFoundImmediately e => e.ExpectedLexemeType,
    // };

    public string Found { get; } = editor.Document.Text.Substring(error.Span.Start, error.Span.Count);
}