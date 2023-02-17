using System;
using System.Globalization;
using System.Windows.Data;
using Downloader;
using MaterialDesignThemes.Wpf;

namespace GoProPilot.Converters;

public class DownloadStateToIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DownloadStatus status)
            return status switch
            {
                DownloadStatus.Running => PackIconKind.Play,
                DownloadStatus.Completed => PackIconKind.Success,
                DownloadStatus.Failed => PackIconKind.Error,
                _ => PackIconKind.Download,
            };

        return PackIconKind.Download;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
