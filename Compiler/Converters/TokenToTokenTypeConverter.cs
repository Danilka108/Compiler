using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Scanner;

namespace Compiler.Converters;

public class TokenToTokenTypeConverter : IValueConverter
{
    private static IDictionary<TokenType, string> TokenTypes = new Dictionary<TokenType, string>
    {
        { TokenType.Ampersand, Lang.Resources.TokenTypeAmpersand },
        { TokenType.ConstKeyword, Lang.Resources.TokenTypeConstKeyword },
        { TokenType.Identifier, Lang.Resources.TokenTypeIdentifier },
        { TokenType.OperatorEnd, Lang.Resources.TokenTypeOperatorEnd },
        { TokenType.AssignmentOperator, Lang.Resources.TokenTypeAssignmentOperator },
        { TokenType.Colon, Lang.Resources.TokenTypeColon },
        { TokenType.Separator, Lang.Resources.TokenTypeSeparator },
        { TokenType.StringLiteral, Lang.Resources.TokenTypeStringLiteral },
        { TokenType.StrKeyword, Lang.Resources.TokenTypeStrKeyword }
    };

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Token<TokenType, TokenError> token) return BindingNotification.UnsetValue;

        return token switch
        {
            Token<TokenType, TokenError>.ValidToken validToken => TokenTypes[validToken.Type],
            Token<TokenType, TokenError>.InvalidToken => "invalid token"
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return BindingNotification.UnsetValue;
    }
}