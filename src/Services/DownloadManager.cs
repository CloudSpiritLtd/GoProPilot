using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using Avalonia.Controls;
using Avalonia.Threading;
using Downloader;
using DryIoc;
using GoProPilot.ViewModels;

namespace GoProPilot.Services;

public class DownloadManager
{
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
        Items.Add(item);
        StartDownload();
    }

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
            Items.Remove(item);
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

    private void OnDownloadFileCompleted(object? sender, AsyncCompletedEventArgs e)
    {
        // todo: test e.Cancelled, e.Error
        // move to next
        if (Current != null)
        {
            UnBindEvents(Current);
            _dispatcher.InvokeAsync(() =>
            {
                Items.Remove(Current);
            });
        }

        StartDownload();
    }

    private void StartDownload()
    {
        if (Items.Count > 0)
        {
            Current = Items[0];
            BindEvents(Current);

            if (!Design.IsDesignMode)
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

    public ObservableCollection<DownloadItem> Items { get; } = new();
}
