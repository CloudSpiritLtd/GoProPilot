using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GoProPilot.Models;

namespace GoProPilot.Converters;

public class MediaFileToDownloadUrlConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var dir = (string)values[0];
        var file = (RawMediaFile)values[1];
        return new Uri($"http://10.5.5.9:8080/videos/DCIM/{dir}/{file.Name}");
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
