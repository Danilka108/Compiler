using System;
using CodeAnalysis;

namespace Compiler.ArithmeticExpr;

public enum LexemeType
{
    OpenBracket,
    CloseBracket,
    Number,
    Plus,
    Dash,
    Asterisk,
    Slash,
    Separator,
    UnexpectedSymbol,
}

public class Lexer() : Lexer<LexemeType>(new LexemeUtils(), Eaters)
{
    private static LexemeEater<LexemeType>[] Eaters =
    [
        TryEatNumber,
        TryEatBracket,
        TryEatOperator,
        TryEatSeparator,
    ];

    private static LexemeType? TryEatBracket(Eater eater)
    {
        if (eater.Eat("(")) return LexemeType.OpenBracket;
        if (eater.Eat(")")) return LexemeType.CloseBracket;

        return null;
    }

    private static LexemeType? TryEatNumber(Eater eater)
    {
        return eater.EatWhile(char.IsDigit) ? LexemeType.Number : null;
    }

    private static LexemeType? TryEatOperator(Eater eater)
    {
        if (eater.Eat("+")) return LexemeType.Plus;
        if (eater.Eat("-")) return LexemeType.Dash;
        if (eater.Eat("*")) return LexemeType.Asterisk;
        if (eater.Eat("/")) return LexemeType.Slash;

        return null;
    }

    private static LexemeType? TryEatSeparator(Eater eater)
    {
        return eater.EatWhile(IsSeparator) ? LexemeType.Separator : null;
    }

    private static bool IsSeparator(char sym, char? nextSym)
    {
        // TODO fix
        return char.IsSeparator(sym)
               || $"{sym}" == Environment.NewLine
               || (nextSym is { } n && $"{sym}{n}" == Environment.NewLine);
    }
}

public class LexemeUtils : ILexemeUtils<LexemeType>
{
    public LexemeType UnexpectedSymbol()
    {
        return LexemeType.UnexpectedSymbol;
    }

    public bool IsIgnorableLexeme(LexemeType lexeme)
    {
        return lexeme is LexemeType.Separator;
    }

    public bool RemoveInvalidLexeme(LexemeType lexeme)
    {
        return false;
    }

    public bool IsInvalidLexeme(LexemeType lexeme)
    {
        return lexeme is LexemeType.UnexpectedSymbol;
    }

    public string LexemeMissingValue(LexemeType lexeme)
    {
        return lexeme switch
        {
            LexemeType.OpenBracket => "(",
            LexemeType.CloseBracket => ")",
            LexemeType.Number => "1",
            LexemeType.Plus => "+",
            LexemeType.Dash => "-",
            LexemeType.Asterisk => "*",
            LexemeType.Slash => "/",
            LexemeType.Separator => " ",
            LexemeType.UnexpectedSymbol => "@",
        };
    }
}