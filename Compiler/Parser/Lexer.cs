using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler.Parser;

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

    public static IEnumerable<Lexeme> Scan(string content)
    {
        var caret = new Caret(content);
        var lexemes = new List<Lexeme>();

        while (!caret.IsEnd())
        {
            var lexeme = LexemeEaters
                .Select(lexemeEater => EatLexeme(caret, lexemeEater))
                .OfType<Lexeme>()
                .FirstOrDefault();

            if (lexeme is not null)
            {
                lexemes.Add(lexeme);
            }
            else
            {
                lexemes.Add(InvalidLexemeType.UnexpectedSymbol.IntoLexeme(caret.Span().ShiftEnd(1)));
                caret.Move();
            }
        }

        return lexemes;
    }

    private static Lexeme? OnNextIteration(Caret caret)
    {
        return LexemeEaters.Select(lexemeEater => EatLexeme(caret, lexemeEater)).OfType<Lexeme>().FirstOrDefault();
        // foreach (var lexemeEater in LexemeEaters)
        // {
        //     var newLexeme = EatLexeme(caret, lexemeEater);
        //     // if (newLexemes.Count > 0) return newLexemes;
        //     if (newLexeme is not null) return newLexeme;
        // }
        //
        // // caret.Move();
        // return null;
    }

    private static Lexeme? EatLexeme(Caret caret, LexemeEater eatFunc)
    {
        var eater = caret.StartEating();

        try
        {
            return eatFunc(eater) is { } lexemeType
                ? lexemeType.IntoLexeme(caret.FinishEating(eater).NewSpan)
                : null;
        }
        catch (EatException ex)
        {
            return ex.Error.IntoLexeme(caret.FinishEating(eater).NewSpan);
        }
    }

    private static List<Lexeme> HandleEatingResult(Caret.EatingResult eatingResult, LexemeType lexemeType)
    {
        var lexeme = lexemeType.IntoLexeme(eatingResult.NewSpan);

        if (eatingResult.OldSpan.IsNotEmpty())
            return [InvalidLexemeType.UnexpectedSymbol.IntoLexeme(eatingResult.OldSpan), lexeme];

        return [lexeme];
    }

    // private static List<Lexeme> EatLexeme(Caret caret, LexemeEater eatFunc)
    // {
    //     var eater = caret.StartEating();
    //
    //     try
    //     {
    //         return eatFunc(eater) is { } lexemeType
    //             ? HandleEatingResult(caret.FinishEating(eater), lexemeType)
    //             : [];
    //     }
    //     catch (EatException ex)
    //     {
    //         return [ex.Error.IntoLexeme(caret.FinishEating(eater).NewSpan)];
    //     }
    // }
    //
    // private static List<Lexeme> HandleEatingResult(Caret.EatingResult eatingResult, LexemeType lexemeType)
    // {
    //     var lexeme = lexemeType.IntoLexeme(eatingResult.NewSpan);
    //
    //     if (eatingResult.OldSpan.IsNotEmpty())
    //         return [InvalidLexemeType.UnexpectedSymbol.IntoLexeme(eatingResult.OldSpan), lexeme];
    //
    //     return [lexeme];
    // }

    private class EatException : Exception
    {
        public InvalidLexemeType Error { get; init; }
    }

    private static LexemeType? TryEatStringLiteral(Caret.Eater eater)
    {
        if (!eater.Eat('"')) return null;
        eater.EatWhile(IsStringLiteralEnd(eater));

        if (!eater.Eat('"')) throw new EatException { Error = InvalidLexemeType.UnterminatedString };
        return LexemeType.StringLiteral;
    }

    private static Func<char, bool> IsStringLiteralEnd(Caret.Eater eater)
    {
        return symbol =>
        {
            eater.Eat('\\', '"');
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