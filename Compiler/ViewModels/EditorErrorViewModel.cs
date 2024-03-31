using Compiler.Parser;

namespace Compiler.ViewModels;

public class EditorErrorViewModel : ViewModelBase
{
    public CaretPos CaretPos { get; init; }

    // public InvalidLexemeType InvalidLexemeType { get; init; }
    public ParseErrorType ErrorType { get; init; }

    public Span Span { get; init; }
}