using System;
using System.ComponentModel;
using Avalonia.Controls;
using Downloader;
using DryIoc;
using GoProPilot.Services;
using ReactiveUI.Fody.Helpers;

namespace GoProPilot.ViewModels;

public class DownloadViewModel : ViewModelBase
{
    public DownloadViewModel()
    {
        DownloadManager = Globals.Container.Resolve<DownloadManager>();
        if (Design.IsDesignMode)
        {
            DownloadManager.Add(new()
            {
                FileName = "GoPro1.mp4",
                Url = "http://test.com/gopro1.mp4",
            });
            DownloadManager.Add(new()
            {
                FileName = "GoPro2.mp4",
                Url = "http://test.com/gopro2.mp4",
            });
        }
    }

    public void AddTask(string name, string url)
    {
        var downloadItem = new DownloadItem
        {
            FileName = name,
            Url = url,
        };

        DownloadManager.Add(downloadItem);
    }

    public DownloadManager DownloadManager { get; }
}

public class DownloadItem : ViewModelBase
{
    public void OnDownloadFileCompleted(object? sender, AsyncCompletedEventArgs e)
    {
        // todo: test e.Cancelled, e.Error
        Status = DownloadStatus.Completed;
    }

    public void OnDownloadProgressChanged(object? sender, DownloadProgressChangedEventArgs e)
    {
        Progress = (int)e.ProgressPercentage;
    }

    public void OnDownloadStarted(object? sender, DownloadStartedEventArgs e)
    {
        Status = DownloadStatus.Running;
    }

    public string FileName { get; init; } = default!;

    [Reactive]
    public int Progress { get; set; } = 0;

    [Reactive]
    public DownloadStatus Status { get; set; } = DownloadStatus.None;

    public string Url { get; init; } = default!;
}
