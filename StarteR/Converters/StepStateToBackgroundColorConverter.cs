using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Color = Avalonia.Media.Color;

namespace StarteR.Converters;

public class StepStateToBackgroundColorConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (!targetType.IsAssignableTo(typeof(IBrush)) || values.Count != 2)
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);

        var isEnabled = values[0] as bool? ?? true;
        var isRunning = values[1] as bool? ?? false;

        if (!isEnabled)
            return new ImmutableSolidColorBrush(new Color(128, 0, 0, 0));
        
        if (isRunning)
            return new ImmutableSolidColorBrush(new Color(32, 0, 255, 0));
        
        return new ImmutableSolidColorBrush(new Color(50, 0, 0, 0));
    }
    
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}