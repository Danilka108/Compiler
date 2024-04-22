using System;
using CodeAnalysis;

namespace Compiler.ConstExpr;

public enum LexemeType
{
    UnexpectedSymbol = 1,
    ConstKeyword,
    StrKeyword,
    Identifier,
    StringLiteral,

    // UninitiatedStringLiteral,
    UnterminatedStringLiteral,
    Colon,
    Ampersand,
    Equal,
    Semicolon,
    Separator,
}

public class Lexer() : Lexer<LexemeType>(new LexemeUtils(), LexemeEaters.Eaters);

public static class LexemeEaters
{
    public static LexemeEater<LexemeType>[] Eaters =
    [
        TryEatAmpersand,
        TryEatColon,
        TryEatAssignOperator,
        TryEatSemicolon,
        TryEatConstKeyword,
        TryEatStrKeyword,
        TryEatStringLiteral,
        TryEatUnterminatedStringLiteral,
        // TryEatUninitiedStringLiteral,
        TryEatIdentifier,
        TryEatSeparator,
    ];

    private static LexemeType? TryEatStringLiteral(Eater eater)
    {
        if (!eater.Eat('"')) return null;
        eater.EatWhile(IsNotStringLiteralEnd(eater));
        if (!eater.Eat('"')) return null;

        return LexemeType.StringLiteral;
    }

    // private static LexemeType? TryEatUninitiedStringLiteral(Eater eater)
    // {
    //     eater.EatWhile(IsNotStringLiteralEnd(eater));
    //     if (!eater.Eat('"')) return null;
    //
    //     return LexemeType.UninitiatedStringLiteral;
    // }

    private static LexemeType? TryEatUnterminatedStringLiteral(Eater eater)
    {
        if (!eater.Eat('"')) return null;
        eater.EatWhile(IsNotUnterminatedStringLiteralEnd(eater));
        // if (!eater.Eat('"')) return null;

        return LexemeType.UnterminatedStringLiteral;
    }

    private static Func<char, char?, bool> IsNotUnterminatedStringLiteralEnd(Eater eater)
    {
        return (symbol, nextSymbol) =>
        {
            _ = eater.Eat('\\', '"');
            return !(IsEol(symbol, nextSymbol) || symbol == ';');
        };

        bool IsEol(char s1, char? s2) => s1 == '\n' || (s1 == '\r' && s2 is '\n');
    }

    private static Func<char, bool> IsNotStringLiteralEnd(Eater eater)
    {
        return symbol =>
        {
            _ = eater.Eat('\\', '"');
            return symbol != '"';
        };
    }

    private static LexemeType? TryEatIdentifier(Eater eater)
    {
        if (!eater.Eat(IsIdentifierHead)) return null;
        eater.EatWhile(IsIdentifierTail);

        return LexemeType.Identifier;
    }

    private static bool IsIdentifierHead(char sym)
    {
        return char.IsLetter(sym) || sym == '_';
    }

    private static bool IsIdentifierTail(char sym)
    {
        return char.IsLetterOrDigit(sym) || sym == '_';
    }

    private static LexemeType? TryEatConstKeyword(Eater eater)
    {
        return eater.Eat("const") ? LexemeType.ConstKeyword : null;
    }

    private static LexemeType? TryEatStrKeyword(Eater eater)
    {
        return eater.Eat("str") ? LexemeType.StrKeyword : null;
    }

    private static LexemeType? TryEatAmpersand(Eater eater)
    {
        return eater.Eat('&') ? LexemeType.Ampersand : null;
    }

    private static LexemeType? TryEatColon(Eater eater)
    {
        return eater.Eat(':') ? LexemeType.Colon : null;
    }

    private static LexemeType? TryEatAssignOperator(Eater eater)
    {
        return eater.Eat('=') ? LexemeType.Equal : null;
    }

    private static LexemeType? TryEatSemicolon(Eater eater)
    {
        return eater.Eat(';') ? LexemeType.Semicolon : null;
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
        return lexeme is not LexemeType.UnterminatedStringLiteral;
    }

    public bool IsInvalidLexeme(LexemeType lexeme)
    {
        return lexeme is LexemeType.UnexpectedSymbol or LexemeType.UnterminatedStringLiteral;
    }

    public string LexemeMissingValue(LexemeType lexeme)
    {
        return lexeme switch
        {
            LexemeType.ConstKeyword => "const",
            LexemeType.StrKeyword => "str",
            LexemeType.Identifier => "ident",
            LexemeType.StringLiteral => "\"string literal\"",
            LexemeType.Colon => ":",
            LexemeType.Ampersand => "&",
            LexemeType.Equal => "=",
            LexemeType.Semicolon => ";",
            _ => throw new ArgumentException(
                $"${LexemeType.UnexpectedSymbol}, ${LexemeType.UnterminatedStringLiteral} and ${LexemeType.Separator} are invalid arguments"),
        };
    }
}