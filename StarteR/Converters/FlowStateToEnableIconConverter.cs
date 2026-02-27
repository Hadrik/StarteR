using System;
using System.Globalization;
using Avalonia.Data.Converters;
using FluentAvalonia.UI.Controls;

namespace StarteR.Converters;

public class FlowStateToEnableIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool isEnabled)
            return null;

        return isEnabled
            ? new SymbolIconSource
            {
                Symbol = Symbol.Cancel
            }
            : new SymbolIconSource
            {
                Symbol = Symbol.Checkmark
            };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}