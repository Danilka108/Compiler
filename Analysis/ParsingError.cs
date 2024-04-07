using Optional;

namespace Analysis;

public enum ParsingErrorKind
{
    LexemeExpected,
    InvalidLexeme,
}

public class ParsingError<TLexemeType> where TLexemeType : struct, Enum
{
    public Span Span { get; internal init; }

    public int TailStart { get; internal init; }

    public TLexemeType? Lexeme { get; internal init; }

    public ParsingErrorKind ErrorKind { get; internal init; }

    internal ParsingError<TLexemeType> SetEnd(int end)
    {
        return new ParsingError<TLexemeType>
        {
            Span = new Span(Span.Start, end),
            TailStart = end,
            Lexeme = Lexeme,
            ErrorKind = ErrorKind,
        };
    }
}