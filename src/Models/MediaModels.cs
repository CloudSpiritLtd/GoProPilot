using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Windows;
using GoProPilot.Services;
using GoProPilot.ViewModels;
using ReactiveUI;

namespace GoProPilot.Models;

/// <summary>
/// A file, with chapters if it is splitted.
/// </summary>
public class ChapteredFile : ViewModelBase
{
    public ChapteredFile()
    {
        DeleteCommand = ReactiveCommand.Create(ExecuteDelete);
    }

    private async void ExecuteDelete()
    {
        if (Deleted
            || MessageBox.Show("Are you sure to delete?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            return;

        await WebApi.Instance.DeleteMediaFiles(Dir, Files.Select(_ => _.Name));
        Deleted = true;
    }

    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

    public bool Deleted { get; set; }

    public string Dir { get; init; } = "";

    public string FileId { get; init; } = "";

    public IEnumerable<RawMediaFile> Files { get; init; } = Array.Empty<RawMediaFile>();

    public RawMediaFile? First
    {
        get { return Files.Any() ? Files.First() : null; }
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
