using System;
using CodeAnalysis;

namespace Compiler.UnsignedNumber;

public enum LexemeType
{
    Digit,
    Plus,
    Dash,
    Dot,
    Ten,
    Separator,
    UnexpectedSymbol,
}

public class Lexer() : Lexer<LexemeType>(new LexemeUtils(), Eaters)
{
    private static readonly LexemeEater<LexemeType>[] Eaters =
    [
        TryEatPlus,
        TryEatDash,
        TryEatDot,
        TryEatTen,
        TryEatDigit,
        TryEatSeparator,
    ];

    private static LexemeType? TryEatDigit(Eater eater)
    {
        return eater.Eat(char.IsDigit) ? LexemeType.Digit : null;
    }

    private static LexemeType? TryEatPlus(Eater eater)
    {
        return eater.Eat('+') ? LexemeType.Plus : null;
    }

    private static LexemeType? TryEatDash(Eater eater)
    {
        return eater.Eat('-') ? LexemeType.Dash : null;
    }

    private static LexemeType? TryEatDot(Eater eater)
    {
        return eater.Eat('.') ? LexemeType.Dot : null;
    }

    private static LexemeType? TryEatTen(Eater eater)
    {
        return eater.Eat("10") ? LexemeType.Ten : null;
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
            LexemeType.Digit => "0",
            LexemeType.Dot => ".",
            LexemeType.Plus => "+",
            LexemeType.Dash => "-",
            LexemeType.Ten => "10",
            LexemeType.Separator => " ",
            LexemeType.UnexpectedSymbol => "@",
        };
    }
}