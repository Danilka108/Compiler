namespace Compiler.Parsing;

public interface ILexemeChecker<in TLexemeType>
{
    bool IsBoundaryLexeme(TLexemeType lexeme);

    bool IsIgnorableLexeme(TLexemeType lexeme);

    bool IsInvalidLexeme(TLexemeType lexeme);
}