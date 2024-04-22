using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Compiler.ConstExpr;

namespace Compiler.Converters;

public class LexemeTypeConverter : IValueConverter
{
    public static readonly IDictionary<LexemeType, string> LexemeTypes =
        new Dictionary<LexemeType, string>
        {
            { LexemeType.ConstKeyword, Lang.Resources.LexemeTypeConstKeyword },
            { LexemeType.Identifier, Lang.Resources.LexemeTypeIdentifier },
            { LexemeType.Colon, Lang.Resources.LexemeTypeColon },
            { LexemeType.Ampersand, Lang.Resources.LexemeTypeAmpersand },
            { LexemeType.StrKeyword, Lang.Resources.LexemeTypeStrKeyword },
            { LexemeType.Equal, Lang.Resources.LexemeTypeEqual },
            { LexemeType.StringLiteral, Lang.Resources.LexemeTypeStringLiteral },
            {
                LexemeType.UnterminatedStringLiteral,
                Lang.Resources.LexemeTypeUnterminatedStringLiteral
            },
            { LexemeType.Semicolon, Lang.Resources.LexemeTypeSemicolon },
            { LexemeType.Separator, Lang.Resources.LexemeTypeSeparator },
            { LexemeType.UnexpectedSymbol, Lang.Resources.LexemeTypeUnexpectedSymbol },
        };

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null) return "";

        if (value is not LexemeType lexemeType)
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);

        return LexemeTypes[lexemeType];
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return BindingNotification.UnsetValue;
    }
}