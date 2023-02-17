using System;
using System.ComponentModel;
using Downloader;
using DryIoc;
using DynamicData;
using GoProPilot.Models;
using DownloaderSvc = Downloader.DownloadService;

namespace GoProPilot.Services;

public class DownloadService
{
    private readonly ConfigService _cfgSvc;
    private readonly DownloaderSvc _downloader;
    private readonly SourceCache<IDownloadItem, string> _items = new(_ => _.FileName);

    public DownloadService()
    {
        _cfgSvc = Core.Container.Resolve<ConfigService>();
        var cfg = new DownloadConfiguration
        {
            ReserveStorageSpaceBeforeStartingDownload= true,
        };
        _downloader = new(cfg);
        _downloader.DownloadFileCompleted += OnDownloadFileCompleted;
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
        Core.MainThreadInvokeAsync(() =>
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
            await Core.MainThreadInvokeAsync(() =>
            {
                _items.Remove(Current);
            });

            Current = null;
        }

        StartDownload();
    }

    private void StartDownload()
    {
        if (Current == null && _items.Count > 0)
        {
            Current = _items.Items.First();
            BindEvents(Current);

#if DEBUG
            if (!Core.IsDesignMode)
#endif
            {
                _downloader.DownloadFileTaskAsync(Current.Url, new DirectoryInfo(_cfgSvc.Config.DownloadFolder));
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
