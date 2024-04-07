namespace Compiler.Parsing;

public enum ParsingErrorKind
{
    LexemeExpected,
    InvalidLexeme,
}

public class ParsingError<TLexemeType>
{
    public Span Span { get; init; }

    public int TailStart { get; init; }

    public TLexemeType? Lexeme { get; init; }

    public ParsingErrorKind ErrorKind { get; init; }
}