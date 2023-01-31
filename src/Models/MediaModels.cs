using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Windows;
using Downloader;
using DryIoc;
using GoProPilot.Services;
using GoProPilot.ViewModels;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace GoProPilot.Models;

/// <summary>
/// A file, with chapters if it is splitted.
/// </summary>
public class ChapteredFile : ViewModelBase
{
    private IEnumerable<RawMediaFile> _rawFiles = Array.Empty<RawMediaFile>();

    public ChapteredFile()
    {
        DeleteCommand = ReactiveCommand.Create(ExecuteDelete);
    }

    private async void ExecuteDelete()
    {
        if (Deleted
            || MessageBox.Show("Are you sure to delete?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            return;

        await WebApi.Instance.DeleteMediaFiles(Dir, Files.Select(_ => _.Raw.Name));
        Deleted = true;
    }

    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

    public bool Deleted { get; set; }

    public string Dir { get; init; } = "";

    public string FileId { get; init; } = "";

    public IEnumerable<RawMediaFile> RawFiles
    {
        get => _rawFiles;
        set { 
            _rawFiles= value;
            Files.Clear();
            foreach (var item in value)
            {
                Files.Add(new MediaFile(Dir, item));
            }
        }
    }

    public IList<MediaFile> Files { get; init; } = new List<MediaFile>();

    public MediaFile? First
    {
        get { return Files.Count > 0 ? Files[0] : null; }
    }
}

/// <summary>
/// A list of files, which are with same date.
/// </summary>
public class FileListByDate
{
    public DateTime Date { get; init; }

    public IEnumerable<ChapteredFile> Files { get; init; } = Array.Empty<ChapteredFile>();
}

/// <summary>
/// A directory of files, which are grouped by date.
/// </summary>
public class MediaDirectory
{
    public string Dir { get; init; } = "";

    public IEnumerable<FileListByDate> FileLists { get; init; } = Array.Empty<FileListByDate>();
}

public class MediaFile : ViewModelBase, IDownloadItem
{
    private readonly DownloadViewModel _downloadVM;
    private string _url;

    public MediaFile(string dir, RawMediaFile raw)
    {
        _downloadVM = Globals.Container.Resolve<DownloadViewModel>();
        _url = $"http://10.5.5.9:8080/videos/DCIM/{dir}/{raw.Name}";
        Raw = raw;
        DownloadCommand = ReactiveCommand.Create(ExecuteDownload);
    }

    private void ExecuteDownload()
    {
        _downloadVM.AddTask(this);
    }

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

    public ReactiveCommand<Unit, Unit> DownloadCommand { get; }

    public string FileName { get => Raw.Name; }

    [Reactive]
    public int Progress { get; set; } = 0;

    [Reactive]
    public DownloadStatus Status { get; set; } = DownloadStatus.None;

    public string Url { get => _url; }


    public RawMediaFile Raw { get; }
}
