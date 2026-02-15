using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace StarteR.Converters;

public class IsGreaterThanZeroConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not IConvertible)
        {
            throw new ArgumentException("Value must be convertible to double.", nameof(value));
        }
        
        return System.Convert.ToDouble(value) > 0.0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}