namespace Analysis.Lexing;

internal class Caret
{
    private readonly string _content;
    private Span _span;

    internal Caret(string content)
    {
        _content = content;
        _span = new Span();
    }

    public Eater StartEating()
    {
        return new Eater(_content, _span);
    }

    public EatingResult FinishEating(Eater eater)
    {
        var result = new EatingResult
        {
            OldSpan = _span,
            NewSpan = eater.Span
        };

        _span = eater.Span.ShiftStartToEnd();
        return result;
    }

    public struct EatingResult
    {
        public Span OldSpan { get; init; }
        public Span NewSpan { get; init; }
    }

    public void Move()
    {
        _span = _span.ShiftEnd(1).ShiftStartToEnd();
    }

    public Span Span()
    {
        return _span;
    }

    public bool IsEnd()
    {
        return _span.End == _content.Length;
    }
}