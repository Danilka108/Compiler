using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Compiler.parser;
using Scanner;

namespace Compiler.Converters;

public class LexemeToLexemeDescriptionConverter : IValueConverter
{
    private static IDictionary<LexemeType, string> TokenTypes = new Dictionary<LexemeType, string>
    {
        { LexemeType.Ampersand, Lang.Resources.TokenTypeAmpersand },
        { LexemeType.ConstKeyword, Lang.Resources.TokenTypeConstKeyword },
        { LexemeType.Identifier, Lang.Resources.TokenTypeIdentifier },
        { LexemeType.OperatorEnd, Lang.Resources.TokenTypeOperatorEnd },
        { LexemeType.AssignmentOperator, Lang.Resources.TokenTypeAssignmentOperator },
        { LexemeType.Colon, Lang.Resources.TokenTypeColon },
        { LexemeType.Separator, Lang.Resources.TokenTypeSeparator },
        { LexemeType.StringLiteral, Lang.Resources.TokenTypeStringLiteral },
        { LexemeType.StrKeyword, Lang.Resources.TokenTypeStrKeyword }
    };

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Lexeme token) return BindingNotification.UnsetValue;

        return token switch
        {
            Lexeme.Valid validToken => TokenTypes[validToken.Type],
            Lexeme.Invalid => "invalid token"
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return BindingNotification.UnsetValue;
    }
}