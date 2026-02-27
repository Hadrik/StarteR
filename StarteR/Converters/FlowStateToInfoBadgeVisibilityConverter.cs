using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;

namespace StarteR.Converters;

public class FlowStateToInfoBadgeVisibilityConverter : IMultiValueConverter
{
    public object? Convert(IList<object?>? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null || value.Count < 2)
            return false;

        if (value[0] is not bool isEnabled || value[1] is not int errorCount)
            return false;

        return !isEnabled || errorCount > 0;
    }
}