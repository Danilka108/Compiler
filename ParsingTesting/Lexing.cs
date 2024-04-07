using Optional;
using Analysis.Lexing;

namespace ParsingTesting;

public enum LexemeType
{
    AccessModifier = 1,
    StructKeyword,
    DataType,
    Identifier,
    OpenBracket,
    CloseBracket,
    EndOperator,
    UnexpectedSymbol,
    Separator,
    // UnexpectedSymbol = 1,
    // ConstKeyword,
    // StrKeyword,
    // Identifier,
    // StringLiteral,
    // UnterminatedStringLiteral,
    // Colon,
    // Ampersand,
    // AssignmentOperator,
    // OperatorEnd,
    // Separator,
}

public class LexemeHelper : ILexemeHelper<LexemeType>
{
    public LexemeType UnexpectedSymbol()
    {
        return LexemeType.UnexpectedSymbol;
    }

    public bool IsIgnorableLexeme(LexemeType lexeme)
    {
        return lexeme is LexemeType.Separator;
    }

    public bool IsInvalidLexeme(LexemeType lexeme)
    {
        return lexeme is LexemeType.UnexpectedSymbol;
    }
}

public static class LexemeEaters
{
    public static Lexer<LexemeType>.LexemeEater[] Eaters =
    [
        TryEatAccessModifier,
        TryEatStructKeyword,
        TryEatDataType,
        TryEatIdentifier,
        TryEatOpenBracket,
        TryEatCloseBracket,
        TryEatEndOperator,
        TryEatSeparator,
    ];

    private static Option<LexemeType> TryEatIdentifier(Eater eater)
    {
        if (!eater.Eat(IsIdentifierHead)) return Option.None<LexemeType>();
        eater.EatWhile(IsIdentifierTail);

        return Option.Some(LexemeType.Identifier);
    }

    private static bool IsIdentifierHead(char sym)
    {
        return char.IsLetter(sym) || sym == '_';
    }

    private static bool IsIdentifierTail(char sym)
    {
        return char.IsLetterOrDigit(sym) || sym == '_';
    }

    private static Option<LexemeType> TryEatDataType(Eater eater)
    {
        return eater.Eat("int") || eater.Eat("char") || eater.Eat("string") || eater.Eat("bool")
            ? Option.Some(LexemeType.DataType)
            : Option.None<LexemeType>();
    }

    private static Option<LexemeType> TryEatAccessModifier(Eater eater)
    {
        return eater.Eat("private") || eater.Eat("public")
            ? Option.Some(LexemeType.AccessModifier)
            : Option.None<LexemeType>();
    }

    private static Option<LexemeType> TryEatStructKeyword(Eater eater)
    {
        return eater.Eat("struct")
            ? Option.Some(LexemeType.StructKeyword)
            : Option.None<LexemeType>();
    }

    private static Option<LexemeType> TryEatOpenBracket(Eater eater)
    {
        return eater.Eat('{') ? Option.Some(LexemeType.OpenBracket) : Option.None<LexemeType>();
    }

    private static Option<LexemeType> TryEatCloseBracket(Eater eater)
    {
        return eater.Eat('}') ? Option.Some(LexemeType.CloseBracket) : Option.None<LexemeType>();
    }

    private static Option<LexemeType> TryEatEndOperator(Eater eater)
    {
        return eater.Eat(';') ? Option.Some(LexemeType.EndOperator) : Option.None<LexemeType>();
    }

    private static Option<LexemeType> TryEatSeparator(Eater eater)
    {
        return eater.EatWhile(IsSeparator) ? Option.Some(LexemeType.Separator) : Option.None<LexemeType>();
    }

    private static bool IsSeparator(char sym, char? nextSym)
    {
        return char.IsSeparator(sym)
               || $"{sym}" == Environment.NewLine
               || (nextSym is { } n && $"{sym}{n}" == Environment.NewLine);
    }
}

// public class LexemeHelper : ILexemeHelper<LexemeType>
// {
//     public LexemeType UnexpectedSymbol()
//     {
//         return LexemeType.UnexpectedSymbol;
//     }
//
//     public bool IsBoundaryLexeme(LexemeType lexeme)
//     {
//         return false;
//     }
//
//     public bool IsIgnorableLexeme(LexemeType lexeme)
//     {
//         return lexeme is LexemeType.Separator;
//     }
//
//     public bool IsInvalidLexeme(LexemeType lexeme)
//     {
//         return lexeme is LexemeType.UnexpectedSymbol or LexemeType.UnterminatedStringLiteral;
//     }
// }
//
// public static class LexemeEaters
// {
//     public static Lexer<LexemeType>.LexemeEater[] Eaters =
//     [
//         TryEatConstKeyword,
//         TryEatStrKeyword,
//         TryEatIdentifier,
//         TryEatStringLiteral,
//         TryEatAmpersand,
//         TryEatColon,
//         TryEatAssignOperator,
//         TryEatOperatorEnd,
//         TryEatSeparator,
//     ];
//
//     private static Option<LexemeType> TryEatStringLiteral(Eater eater)
//     {
//         if (!eater.Eat('"')) return Option.None<LexemeType>();
//         eater.EatWhile(IsStringLiteralEnd(eater));
//         if (!eater.Eat('"')) return Option.Some(LexemeType.UnterminatedStringLiteral);
//
//         return Option.Some(LexemeType.StringLiteral);
//     }
//
//     private static Func<char, bool> IsStringLiteralEnd(Eater eater)
//     {
//         return symbol =>
//         {
//             var _ = eater.Eat('\\', '"');
//             return symbol != '"';
//         };
//     }
//
//     private static Option<LexemeType> TryEatIdentifier(Eater eater)
//     {
//         if (!eater.Eat(IsIdentifierHead)) return Option.None<LexemeType>();
//         eater.EatWhile(IsIdentifierTail);
//
//         return Option.Some(LexemeType.Identifier);
//     }
//
//     private static bool IsIdentifierHead(char sym)
//     {
//         return char.IsLetter(sym) || sym == '_';
//     }
//
//     private static bool IsIdentifierTail(char sym)
//     {
//         return char.IsLetterOrDigit(sym) || sym == '_';
//     }
//
//     private static Option<LexemeType> TryEatConstKeyword(Eater eater)
//     {
//         return eater.Eat("const") ? Option.Some(LexemeType.ConstKeyword) : Option.None<LexemeType>();
//     }
//
//     private static Option<LexemeType> TryEatStrKeyword(Eater eater)
//     {
//         return eater.Eat("str") ? Option.Some(LexemeType.StrKeyword) : Option.None<LexemeType>();
//     }
//
//     private static Option<LexemeType> TryEatAmpersand(Eater eater)
//     {
//         return eater.Eat('&') ? Option.Some(LexemeType.Ampersand) : Option.None<LexemeType>();
//     }
//
//     private static Option<LexemeType> TryEatColon(Eater eater)
//     {
//         return eater.Eat(':') ? Option.Some(LexemeType.Colon) : Option.None<LexemeType>();
//     }
//
//     private static Option<LexemeType> TryEatAssignOperator(Eater eater)
//     {
//         return eater.Eat('=') ? Option.Some(LexemeType.AssignmentOperator) : Option.None<LexemeType>();
//     }
//
//     private static Option<LexemeType> TryEatOperatorEnd(Eater eater)
//     {
//         return eater.Eat(';') ? Option.Some(LexemeType.OperatorEnd) : Option.None<LexemeType>();
//     }
//
//     private static Option<LexemeType> TryEatSeparator(Eater eater)
//     {
//         return eater.EatWhile(IsSeparator) ? Option.Some(LexemeType.Separator) : Option.None<LexemeType>();
//     }
//
//     private static bool IsSeparator(char sym, char? nextSym)
//     {
//         return char.IsSeparator(sym)
//                || $"{sym}" == Environment.NewLine
//                || (nextSym is { } n && $"{sym}{n}" == Environment.NewLine);
//     }
// }