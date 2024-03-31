using Compiler.Parser;

namespace Compiler.ViewModels;

public class EditorParseErrorViewModel : ViewModelBase
{
    public ParseErrorType ErrorType { get; init; }

    public CaretPos CaretPos { get; init; }

    public Span Span { get; init; }
}