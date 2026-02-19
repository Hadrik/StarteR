using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace StarteR.Converters;

public class StepStateToBackgroundColorConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (!targetType.IsAssignableTo(typeof(IBrush)) || values.Count != 2)
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);

        var isEnabled = values[0] as bool? ?? true;
        var isRunning = values[1] as bool? ?? false;
        var app = Application.Current!;

        if (!isEnabled)
        {
            app.TryGetResource("CardDisabledBrush", out var disabledBrush);
            return disabledBrush;
        }

        if (isRunning)
        {
            app.TryGetResource("CardRunningBrush", out var runningBrush);
            return runningBrush;
        }
        
        app.TryGetResource("CardBackgroundBrush", out var defaultBrush);
        return defaultBrush;
    }
    
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}