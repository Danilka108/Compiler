using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Compiler.ViewModels;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Compiler.Converters;

public class CaretPosConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not CaretPos caretPos)
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);

        return string.Format(Lang.Resources.CaretPosColumnValue, caretPos.Row, caretPos.Column);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return BindingNotification.UnsetValue;
    }
}