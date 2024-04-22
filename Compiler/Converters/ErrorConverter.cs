using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using CodeAnalysis;
using Compiler.ConstExpr;

namespace Compiler.Converters;

public class ErrorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not AnalyzerError<LexemeType> error)
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);

        return error switch
        {
            AnalyzerError<LexemeType>.InvalidLexeme e => string.Format(Lang.Resources
                .ErrorKindInvalidLexeme, LexemeTypeConverter.LexemeTypes[e.InvalidLexemeType]),
            AnalyzerError<LexemeType>.LexemeNotFound => Lang.Resources
                .ErrorKindNotFound,
            AnalyzerError<LexemeType>.LexemeNotFoundImmediately => Lang.Resources
                .ErrorKindNotFoundImmediately,
            AnalyzerError<LexemeType>.LexemesExhausted => Lang.Resources
                .ErrorKindExhausted
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return BindingNotification.UnsetValue;
    }
}