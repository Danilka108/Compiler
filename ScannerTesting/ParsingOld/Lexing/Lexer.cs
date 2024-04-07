using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.Parsing.Lexing;

using LexemeEater = Func<Caret.Eater, LexemeType?>;

public class Lexer
{
    private static readonly LexemeEater[] LexemeEaters =
    [
        TryEatStringLiteral,
        TryEatConstKeyword,
        TryEatStrKeyword,
        TryEatIdentifier,
        TryEatAmpersand,
        TryEatColon,
        TryEatAssignOperator,
        TryEatOperatorEnd,
        TryEatSeparator
    ];

    public static IEnumerable<Compiler.LexemeType> Scan(string content)
    {
        var caret = new Caret(content);
        var lexemes = new List<Compiler.LexemeType>();

        while (!caret.IsEnd())
        {
            var lexeme = LexemeEaters
                .Select(lexemeEater => EatLexeme(caret, lexemeEater))
                .OfType<Compiler.LexemeType>()
                .FirstOrDefault();

            if (lexeme is not null)
            {
                lexemes.Add(lexeme);
            }
            else
            {
                lexemes.Add(LexemeType.UnexpectedSymbol.IntoLexeme(caret.Span().ShiftEnd(1)));
                caret.Move();
            }
        }

        return lexemes;
    }

    private static Compiler.LexemeType? EatLexeme(Caret caret, LexemeEater eatFunc)
    {
        var eater = caret.StartEating();

        if (eatFunc(eater) is { } lexemeType)
        {
            return lexemeType.IntoLexeme(caret.FinishEating(eater).NewSpan);
        }

        return null;
    }

    private static LexemeType? TryEatStringLiteral(Caret.Eater eater)
    {
        if (!eater.Eat('"')) return null;
        eater.EatWhile(IsStringLiteralEnd(eater));
        if (!eater.Eat('"')) return LexemeType.UnterminatedStringLiteral;

        return LexemeType.StringLiteral;
    }

    private static Func<char, bool> IsStringLiteralEnd(Caret.Eater eater)
    {
        return symbol =>
        {
            var _ = eater.Eat('\\', '"');
            return symbol != '"';
        };
    }

    private static LexemeType? TryEatIdentifier(Caret.Eater eater)
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

    private static LexemeType? TryEatConstKeyword(Caret.Eater eater)
    {
        return eater.Eat("const") ? LexemeType.ConstKeyword : null;
    }

    private static LexemeType? TryEatStrKeyword(Caret.Eater eater)
    {
        return eater.Eat("str") ? LexemeType.StrKeyword : null;
    }

    private static LexemeType? TryEatAmpersand(Caret.Eater eater)
    {
        return eater.Eat('&') ? LexemeType.Ampersand : null;
    }

    private static LexemeType? TryEatColon(Caret.Eater eater)
    {
        return eater.Eat(':') ? LexemeType.Colon : null;
    }

    private static LexemeType? TryEatAssignOperator(Caret.Eater eater)
    {
        return eater.Eat('=') ? LexemeType.AssignmentOperator : null;
    }

    private static LexemeType? TryEatOperatorEnd(Caret.Eater eater)
    {
        return eater.Eat(';') ? LexemeType.OperatorEnd : null;
    }

    private static LexemeType? TryEatSeparator(Caret.Eater eater)
    {
        return eater.EatWhile(IsSeparator) ? LexemeType.Separator : null;
    }

    private static bool IsSeparator(char sym, char? nextSym)
    {
        return char.IsSeparator(sym)
               || $"{sym}" == Environment.NewLine
               || (nextSym is { } n && $"{sym}{n}" == Environment.NewLine);
    }
}