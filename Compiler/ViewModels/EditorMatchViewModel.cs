using CodeAnalysis;

namespace Compiler.ViewModels;

public class EditorMatchViewModel(ITextEditor editor, string matching, Span span) : ViewModelBase
{
    public string Matching { get; }

    public Span Span { get; } = span;

    public CaretPos CaretPosStart { get; } = editor.OffsetToCaretPos(span.Start);

    public CaretPos CaretPosEnd { get; } = editor.OffsetToCaretPos(span.End);

    public string Found { get; } = editor.Document.Text.Substring(span.Start, span.Count);
}