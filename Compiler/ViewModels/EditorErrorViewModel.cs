using Scanner;

namespace Compiler.ViewModels;

public class EditorErrorViewModel : ViewModelBase
{
    public CaretPos CaretPos { get; init; }

    public TokenError TokenError { get; init; }

    public Span Span { get; init; }
}