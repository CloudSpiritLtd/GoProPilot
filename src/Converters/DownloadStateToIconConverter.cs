using System;
using System.Globalization;
using System.Windows.Data;
using Downloader;
using FluentAvalonia.UI.Controls;

namespace GoProPilot.Converters;

public class DownloadStateToIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DownloadStatus status)
            return status switch
            {
                DownloadStatus.Running => Symbol.Play,
                DownloadStatus.Completed => Symbol.Checkmark,
                DownloadStatus.Failed => Symbol.Dismiss,
                _ => Symbol.Download,
            };

        return Symbol.Download;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
