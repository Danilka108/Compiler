using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Compiler.ArithmeticExpr;

namespace Compiler.Converters;

public class LexemeTypeConverter : IValueConverter
{
    public static readonly IDictionary<LexemeType, string> LexemeTypes =
        new Dictionary<LexemeType, string>
        {
            { LexemeType.Number, Lang.Resources.LexemeTypeNumber },
            { LexemeType.Plus, Lang.Resources.LexemeTypePlus },
            { LexemeType.Dash, Lang.Resources.LexemeTypeDash },
            { LexemeType.Asterisk, Lang.Resources.LexemeTypeAsterisk },
            { LexemeType.Slash, Lang.Resources.LexemeTypeSlash },
            { LexemeType.CloseBracket, Lang.Resources.LexemeTypeCloseBracket },
            { LexemeType.OpenBracket, Lang.Resources.LexemeTypeOpenBracket },
            { LexemeType.UnexpectedSymbol, Lang.Resources.LexemeTypeUnexpectedSymbol },
            { LexemeType.Separator, Lang.Resources.LexemeTypeSeparator },
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