using Compiler.Parser;

namespace Compiler.ViewModels;

public class EditorErrorViewModel(string content) : ViewModelBase
{
    public CaretPos CaretPos { get; init; }

    // public InvalidLexemeType InvalidLexemeType { get; init; }
    public Parsing.ParsingError Error { get; init; }

    public string Tail => content[Error.TailStart..];

    public Parsing.Span Span { get; init; }
}