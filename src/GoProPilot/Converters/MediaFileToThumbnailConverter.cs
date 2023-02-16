using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using GoProPilot.Models;

namespace GoProPilot.Converters;

public class MediaFileToThumbnailConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
#if DEBUG
        if (Design.IsDesignMode)
        {
            return "../Assets/Mock.Thumbnail.jpg";
        }
#endif
        if (values[0] is string dir && values[1] is RawMediaFile file)
            return $"http://10.5.5.9:8080/gopro/media/thumbnail?path={dir}/{file.Name}";
        else
            return null;
    }
}
