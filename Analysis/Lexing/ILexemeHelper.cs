namespace Analysis.Lexing;

public interface ILexemeHelper<TLexemeType>
{
    TLexemeType UnexpectedSymbol();

    // bool IsBoundaryLexeme(TLexemeType lexeme);

    bool IsIgnorableLexeme(TLexemeType lexeme);

    bool IsInvalidLexeme(TLexemeType lexeme);
}