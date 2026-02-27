using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace StarteR.Converters;

public class ZeroAsUnsetValueConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            int intValue => intValue == 0 ? -1 : intValue,
            double doubleValue => doubleValue == 0.0 ? AvaloniaProperty.UnsetValue : doubleValue,
            _ => value
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}