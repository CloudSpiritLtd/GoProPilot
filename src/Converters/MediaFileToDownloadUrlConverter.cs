using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using GoProPilot.Models;

namespace GoProPilot.Converters;

public class MediaFileToDownloadUrlConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values[0] is string dir && values[1] is RawMediaFile file)
            return $"http://10.5.5.9:8080/videos/DCIM/{dir}/{file.Name}";
        else
            return string.Empty;
    }
}
