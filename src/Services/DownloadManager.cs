using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Threading;
using Downloader;
using DryIoc;
using DynamicData;
using GoProPilot.ViewModels;

namespace GoProPilot.Services;

public class DownloadManager
{
    private readonly SourceCache<DownloadItem, string> _items = new(_ => _.FileName);
    private readonly ConfigService _configService;
    private readonly Dispatcher _dispatcher = Dispatcher.UIThread;
    private readonly DownloadService _downloader = new();

    public DownloadManager()
    {
        _configService = Globals.Container.Resolve<ConfigService>();
        _downloader.DownloadFileCompleted += OnDownloadFileCompleted;
    }

    public void Add(DownloadItem item)
    {
        _items.AddOrUpdate(item);
        StartDownload();
    }

    public IObservable<IChangeSet<DownloadItem, string>> Connect() => _items.Connect();

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

    public void Remove(DownloadItem item)
    {
        UnBindEvents(item);
        _dispatcher.InvokeAsync(() =>
        {
            _items.Remove(item);
        });
    }

    private void BindEvents(DownloadItem item)
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
                _downloader.DownloadFileTaskAsync(Current.Url, new DirectoryInfo(_configService.Config.DownloadFolder));
            }
        }
    }

    private void UnBindEvents(DownloadItem item)
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

    public DownloadItem? Current { get; private set; }
}
