using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Scanner;

namespace Compiler.Converters;

public class TokenErrorToDescriptionConverter : IValueConverter
{
    private static readonly IDictionary<TokenError, string> Matches = new Dictionary<TokenError, string>
    {
        { TokenError.UnexpectedSymbol, Lang.Resources.TokenErrorUnexpectedSymbol },
        { TokenError.UnterminatedString, Lang.Resources.TokenErrorUnterminatedString },
        { TokenError.IdentifierCanOnlyStartWithANumber, Lang.Resources.TokenErrorIdentifierCanOnlyStartWithALetter }
    };

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not TokenError tokenError) return BindingNotification.UnsetValue;

        return Matches[tokenError];
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return BindingNotification.UnsetValue;
    }
}