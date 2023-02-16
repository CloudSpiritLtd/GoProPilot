using System.ComponentModel;
using Downloader;

namespace GoProPilot.Models;

public interface IDownloadItem
{
    void OnDownloadFileCompleted(object? sender, AsyncCompletedEventArgs e);

    void OnDownloadProgressChanged(object? sender, DownloadProgressChangedEventArgs e);

    void OnDownloadStarted(object? sender, DownloadStartedEventArgs e);

    string FileName { get; }

    int Progress { get; set; }

    string Url { get; }
}
