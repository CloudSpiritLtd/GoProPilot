using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Threading;
using DryIoc;
using DynamicData;
using GoProPilot.Models;
using GoProPilot.ViewModels;
using DownloaderSvc = Downloader.DownloadService;

namespace GoProPilot.Services;

public class DownloadService
{
    private readonly SettingsViewModel _settingsVM;
    private readonly Dispatcher _dispatcher = Dispatcher.UIThread;
    private readonly DownloaderSvc _downloader = new();
    private readonly SourceCache<IDownloadItem, string> _items = new(_ => _.FileName);

    public DownloadService()
    {
        _downloader.DownloadFileCompleted += OnDownloadFileCompleted;
        _settingsVM = Globals.Container.Resolve<SettingsViewModel>();
    }

    public void Add(IDownloadItem item)
    {
        _items.AddOrUpdate(item);
        StartDownload();
    }

    public IObservable<IChangeSet<IDownloadItem, string>> Connect() => _items.Connect();

    public void ExecuteCancel()
    {
        _downloader.CancelAsync();
    }

    public void ExecutePause()
    {
        _downloader.Pause();
    }

    public void ExecuteResume()
    {
        _downloader.Resume();
    }

    public void Remove(IDownloadItem item)
    {
        UnBindEvents(item);
        _dispatcher.InvokeAsync(() =>
        {
            _items.Remove(item);
        });
    }

    private void BindEvents(IDownloadItem item)
    {
        // Provide `FileName` and `TotalBytesToReceive` at the start of each downloads
        _downloader.DownloadStarted += item.OnDownloadStarted;

        // Provide any information about chunker downloads,
        // like progress percentage per chunk, speed,
        // total received bytes and received bytes array to live streaming.

        //_downloader.ChunkDownloadProgressChanged += item.OnChunkDownloadProgressChanged;

        // Provide any information about download progress,
        // like progress percentage of sum of chunks, total speed,
        // average speed, total received bytes and received bytes array
        // to live streaming.
        _downloader.DownloadProgressChanged += item.OnDownloadProgressChanged;

        // Download completed event that can include occurred errors or
        // cancelled or download completed successfully.
        _downloader.DownloadFileCompleted += item.OnDownloadFileCompleted;
    }

    private async void OnDownloadFileCompleted(object? sender, AsyncCompletedEventArgs e)
    {
        // todo: test e.Cancelled, e.Error
        // move to next
        if (Current != null)
        {
            UnBindEvents(Current);
            await _dispatcher.InvokeAsync(() =>
            {
                _items.Remove(Current);
            });
        }

        StartDownload();
    }

    private void StartDownload()
    {
        if (_items.Count > 0)
        {
            Current = _items.Items.First();
            BindEvents(Current);

#if DEBUG
            if (!Design.IsDesignMode)
#endif
            {
                _downloader.DownloadFileTaskAsync(Current.Url, new DirectoryInfo(_settingsVM.DownloadFolder));
            }
        }
    }

    private void UnBindEvents(IDownloadItem item)
    {
        // Provide `FileName` and `TotalBytesToReceive` at the start of each downloads
        _downloader.DownloadStarted -= item.OnDownloadStarted;

        // Provide any information about chunker downloads,
        // like progress percentage per chunk, speed,
        // total received bytes and received bytes array to live streaming.

        //_downloader.ChunkDownloadProgressChanged -= item.OnChunkDownloadProgressChanged;

        // Provide any information about download progress,
        // like progress percentage of sum of chunks, total speed,
        // average speed, total received bytes and received bytes array
        // to live streaming.
        _downloader.DownloadProgressChanged -= item.OnDownloadProgressChanged;

        // Download completed event that can include occurred errors or
        // cancelled or download completed successfully.
        _downloader.DownloadFileCompleted -= item.OnDownloadFileCompleted;
    }

    public IDownloadItem? Current { get; private set; }
}
