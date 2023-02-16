using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Linq;
using Avalonia.Controls;
using DryIoc;
using DynamicData;
using GoProPilot.Models;
using GoProPilot.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using DownloadStatus = Downloader.DownloadStatus;

namespace GoProPilot.ViewModels;

public class DownloadViewModel : ViewModelBase
{
    private readonly ReadOnlyObservableCollection<IDownloadItem> _items;

    public DownloadViewModel()
    {
        DownloadService = Globals.Container.Resolve<DownloadService>();
        DownloadService.Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _items)
            .Subscribe();

#if DEBUG
        if (Design.IsDesignMode)
        {
            /*
            DownloadService.Add(new()
            {
                FileName = "GoPro1.mp4",
                Url = "http://test.com/gopro1.mp4",
            });
            DownloadService.Add(new()
            {
                FileName = "GoPro2.mp4",
                Url = "http://test.com/gopro2.mp4",
            });
            */
        }
#endif
    }

    /*
    public void AddTask(string name, string url)
    {
        var downloadItem = new DownloadItem
        {
            FileName = name,
            Url = url,
        };

        DownloadService.Add(downloadItem);
    }
    */

    //public void AddTask(IDownloadItem item) => DownloadService.Add(item);

    public DownloadService DownloadService { get; }

    public ReadOnlyObservableCollection<IDownloadItem> Items { get => _items; }
}

[Obsolete]
public class DownloadItem1 : ViewModelBase, IDownloadItem
{
    public void OnDownloadFileCompleted(object? sender, AsyncCompletedEventArgs e)
    {
        // todo: test e.Cancelled, e.Error
        Status = DownloadStatus.Completed;
    }

    public void OnDownloadProgressChanged(object? sender, Downloader.DownloadProgressChangedEventArgs e)
    {
        Progress = (int)e.ProgressPercentage;
    }

    public void OnDownloadStarted(object? sender, Downloader.DownloadStartedEventArgs e)
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
