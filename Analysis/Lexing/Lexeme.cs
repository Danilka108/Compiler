namespace Analysis.Lexing;

public class Lexeme<TType> where TType : Enum
{
    public Span Span { get; internal init; }

    public TType Type { get; internal init; }
}

public static class LexemeExtensions
{
    public static Lexeme<TType> IntoLexeme<TType>(this TType lexemeType, Span span) where TType : Enum
    {
        return new Lexeme<TType>
        {
            Type = lexemeType,
            Span = span,
        };
    }
}