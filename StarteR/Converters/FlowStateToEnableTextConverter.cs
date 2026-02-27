using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace StarteR.Converters;

public class FlowStateToEnableTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool isEnabled)
            return "?";
        
        return isEnabled ? "Disable" : "Enable";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}