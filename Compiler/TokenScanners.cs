using System;
using System.Collections.Generic;
using Scanner;

namespace Compiler;

using ScanResult = ScanResult<TokenType, TokenError>;
using TokenScanner = TokenScanner<TokenType, TokenError>;

public enum TokenType
{
    ConstKeyword,
    Identifier,
    Colon,
    Ampersand,
    StrKeyword,
    AssignmentOperator,
    StringLiteral,
    OperatorEnd,
    Separator
}

public enum TokenError
{
    UnexpectedSymbol = 0,
    UnterminatedString,
    IdentifierCanOnlyStartWithANumber
}

public static class TokensScanners
{
    public static readonly IEnumerable<TokenScanner> TokenScanners =
        new TokenScanner[]
        {
            caret => ScanOneSymbolToken(caret, ':', TokenType.Colon),
            caret => ScanOneSymbolToken(caret, '&', TokenType.Ampersand),
            caret => ScanOneSymbolToken(caret, '=', TokenType.AssignmentOperator),
            caret => ScanOneSymbolToken(caret, ';', TokenType.OperatorEnd),
            ScanKeywords,
            ScanIdentifier,
            ScanStringLiteralToken,
            ScanSeparatorToken
        };

    private static ScanResult ScanKeywords(Caret caret)
    {
        if (!caret.TryEatWhile(char.IsLetterOrDigit)) return ScanResult.Nothing();

        return caret.Slice() switch
        {
            "const" => ScanResult.Token(TokenType.ConstKeyword),
            "str" => ScanResult.Token(TokenType.StrKeyword),
            _ => ScanResult.Nothing()
        };
    }

    private static ScanResult ScanIdentifier(Caret caret)
    {
        if (!caret.TryEatWhile(char.IsLetterOrDigit)) return ScanResult.Nothing();

        var value = caret.Slice();

        return value.Length > 0 && char.IsLetter(value[0])
            ? ScanResult.Token(TokenType.Identifier)
            : ScanResult.Error(TokenError.IdentifierCanOnlyStartWithANumber);
    }

    private static ScanResult ScanOneSymbolToken(Caret caret, char symbol, TokenType type)
    {
        return caret.TryEat(symbol)
            ? ScanResult.Token(type)
            : ScanResult.Nothing();
    }

    private static ScanResult ScanStringLiteralToken(Caret caret)
    {
        if (!caret.TryEat('"')) return ScanResult.Nothing();

        caret.TryEatWhile(IsStringLiteralEnd(caret));

        return caret.TryEat('"')
            ? ScanResult.Token(TokenType.StringLiteral)
            : ScanResult.Error(TokenError.UnterminatedString);
    }

    private static Func<char, bool> IsStringLiteralEnd(Caret caret)
    {
        return symbol =>
        {
            caret.TryEat('\\', '"');
            return symbol != '"';
        };
    }

    private static ScanResult ScanSeparatorToken(Caret caret)
    {
        return caret.TryEatWhile(char.IsSeparator)
            ? ScanResult.Token(TokenType.Separator)
            : ScanResult.Nothing();
    }
}