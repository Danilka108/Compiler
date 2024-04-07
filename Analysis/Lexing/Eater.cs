namespace Analysis.Lexing;

public class Eater
{
    public Span Span { get; private set; }

    private readonly string _content;

    internal Eater(string content, Span span)
    {
        _content = content;
        Span = span.ShiftStartToEnd();
    }

    // public string EatenContent()
    // {
    //     if (IsOutOfRange()) return "";
    //     return content[Span.Start..int.Min(Span.End, content.Length)];
    // }

    public bool Eat(char symbol)
    {
        if (GetCurrentSymbol() != symbol) return false;

        Span = Span.ShiftEnd(1);
        return true;
    }

    public bool Eat(Func<char, bool> predicate)
    {
        if (GetCurrentSymbol() is { } currentSymbol && predicate(currentSymbol))
        {
            Span = Span.ShiftEnd(1);
            return true;
        }

        return false;
    }

    public bool Eat(char symbol, char nextSymbol)
    {
        if (GetCurrentSymbol() != symbol || GetNextSymbol() != nextSymbol) return false;

        Span = Span.ShiftEnd(2);
        return true;
    }

    public bool Eat(string symbols)
    {
        if (symbols.Where((t, i) => t != GetSymbol(i)).Any())
        {
            return false;
        }

        Span = Span.ShiftEnd(symbols.Length);
        return true;

        // return symbols.All(Eat);

        // if (GetCurrentSymbol() != symbol) return false;
        //
        // Span = Span.ShiftEnd(1);
        // return true;
    }

    public bool EatWhile(Func<char, bool> predicate)
    {
        var start = Span.End;
        while (GetCurrentSymbol() is { } currentSymbol && predicate(currentSymbol)) Span = Span.ShiftEnd(1);
        return Span.End - start > 0;
    }

    public bool EatWhile(Func<char, char?, bool> predicate)
    {
        var start = Span.End;
        while (GetCurrentSymbol() is { } currentSymbol &&
               predicate(currentSymbol, GetNextSymbol())) Span = Span.ShiftEnd(1);
        return Span.End - start > 0;
    }

    private char? GetCurrentSymbol()
    {
        return GetSymbol(shift: 0);
    }

    private char? GetNextSymbol()
    {
        return GetSymbol(shift: 1);
    }

    private char? GetSymbol(int shift)
    {
        if (IsOutOfRange(shift)) return null;
        return _content[Span.End + shift];
    }

    private bool IsOutOfRange()
    {
        return Span.End >= _content.Length;
    }

    private bool IsOutOfRange(int shift)
    {
        return Span.End + shift >= _content.Length;
    }
}