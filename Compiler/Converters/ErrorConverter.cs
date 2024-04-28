using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Compiler.ArithmeticExpr.InfixNotation;

namespace Compiler.Converters;

public class ErrorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not ParseError error)
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);

        return error switch
        {
            ParseError.ExpectedOperator => Lang.Resources.ErrorKindExpectedOperator,
            ParseError.ExpectedOperand => Lang.Resources.ErrorKindExpectedOperand,
            ParseError.ExpectedCloseBracket => Lang.Resources.ErrorKindExpectedCloseBracket,
            ParseError.ExpectedOpenBracket => Lang.Resources.ErrorKindExpectedOpenBracket,
            ParseError.UnexpectedSymbol => string.Format(Lang.Resources
                .ErrorKindInvalidLexeme, ""),
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return BindingNotification.UnsetValue;
    }
}