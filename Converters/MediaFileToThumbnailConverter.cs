using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using GoProPilot.Models;

namespace GoProPilot.Converters;

public class MediaFileToThumbnailConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values[0] is string dir && values[1] is RawMediaFile file)
        {
            var url = $"http://10.5.5.9:8080/gopro/media/thumbnail?path={dir}/{file.Name}";
            return new BitmapImage(new Uri(url));
        }
        else
            return new BitmapImage();
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
