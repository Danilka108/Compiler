namespace Compiler.Parsing.Lexing;

public class Lexeme<TLexemeType>
{
    public Span Span { get; init; }

    public TLexemeType Type { get; init; }
}

public static class LexemeExtensions
{
    public static Lexeme<TLexemeType> IntoLexeme(this LexemeType lexemeType, Span span)
    {
        return new Compiler.LexemeType
        {
            Type = lexemeType,
            Span = span,
        };
    }
}