using Avalonia.Data.Converters;
using System;

namespace Compiler;

public class DefaultFileNameConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        // Проверяем, является ли значение null
        if (value == null)
            // Если null, возвращаем "Untitled"
            return "Untitled";

        // Если не null, возвращаем оригинальное значение
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        // Этот метод не требуется для этого примера
        throw new NotImplementedException();
    }
}