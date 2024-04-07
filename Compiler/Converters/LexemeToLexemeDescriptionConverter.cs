using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Compiler.Parser;
using Scanner;

namespace Compiler.Converters;

public class LexemeToLexemeDescriptionConverter : IValueConverter
{
    private static IDictionary<Parser.LexemeType, string> TokenTypes = new Dictionary<Parser.LexemeType, string>
    {
        { Parser.LexemeType.Ampersand, Lang.Resources.TokenTypeAmpersand },
        { Parser.LexemeType.ConstKeyword, Lang.Resources.TokenTypeConstKeyword },
        { Parser.LexemeType.Identifier, Lang.Resources.TokenTypeIdentifier },
        { Parser.LexemeType.OperatorEnd, Lang.Resources.TokenTypeOperatorEnd },
        { Parser.LexemeType.AssignmentOperator, Lang.Resources.TokenTypeAssignmentOperator },
        { Parser.LexemeType.Colon, Lang.Resources.TokenTypeColon },
        { Parser.LexemeType.Separator, Lang.Resources.TokenTypeSeparator },
        { Parser.LexemeType.StringLiteral, Lang.Resources.TokenTypeStringLiteral },
        { Parser.LexemeType.StrKeyword, Lang.Resources.TokenTypeStrKeyword }
    };

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not LexemeType token) return BindingNotification.UnsetValue;

        return token switch
        {
            LexemeType.Valid validToken => TokenTypes[validToken.Type],
            LexemeType.Invalid => Lang.Resources.InvalidLexeme,
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return BindingNotification.UnsetValue;
    }
}