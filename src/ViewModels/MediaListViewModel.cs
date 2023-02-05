using DryIoc;
using DryIoc.ImTools;
using GoProPilot.Models;
using GoProPilot.Services;
using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Reactive;

namespace GoProPilot.ViewModels;

public class MediaListViewModel : ViewModelBase
{
    private const string MOCK_GET_MEDIA_LIST = "Mock.WebApi.GetMediaList.json";
    private readonly DownloadService _downloadSvc;

    public MediaListViewModel()
    {
        _downloadSvc = Globals.Container.Resolve<DownloadService>();
        DownloadCommand = ReactiveCommand.Create<MediaFile>(ExecuteDownload);
        GetMediasCommand = ReactiveCommand.Create(ExecuteGetMedias);

#if DEBUG
        LoadMockData();
#endif
    }

    private void ExecuteDownload(MediaFile file)
    {
        if (file.Status == Downloader.DownloadStatus.None)
        {
            //todo: delete old file then add task
            _downloadSvc.Add(file);
            file.IsWaitingDownload = true;
        }
    }

    private async void ExecuteGetMedias()
    {
        try
        {
            var str = await WebApi.Instance.GetMediaGroupsAsync();
            Medias = ParseMediaListJson(str);
        }
        catch (Exception ex)
        {
            await Utils.ShowErrorMessageAsync(ex.Message);
        }
    }

#if DEBUG
    private void LoadMockData()
    {
        System.Diagnostics.Debug.WriteLine(Environment.CurrentDirectory);
        if (File.Exists(MOCK_GET_MEDIA_LIST))
        {
            System.Diagnostics.Debug.WriteLine("Found mock file for MediaListView");
            //System.Windows.MessageBox.Show("!");
            using var sr = new StreamReader(MOCK_GET_MEDIA_LIST);
            var str = sr.ReadToEnd();

            Medias = ParseMediaListJson(str);
        }
    }
#endif

    private static MediaDirectory[] ParseMediaListJson(string jsonText)
    {
        var json = JsonConvert.DeserializeObject<RawMediaListResponse>(jsonText);
        if (json == null)
            return Array.Empty<MediaDirectory>();

        var medias = from a in json.Media
                         // One folder in tf card
                     select new MediaDirectory
                     {
                         Dir = a.Directory,
                         FileLists = from b in a.Files
                                     orderby b.Modified descending
                                     group b by b.Modified.Date into g1
                                     // A list of files that taken in same date
                                     select new FileListByDate
                                     {
                                         Date = g1.Key,
                                         Files = from c in g1
                                                 orderby c.Modified descending
                                                 group c by c.Name.Substring(4, 4) into g2
                                                 // A group of files, with same capture serial number
                                                 select new ChapteredFile
                                                 {
                                                     Dir = a.Directory,
                                                     FileId = g2.Key,
                                                     RawFiles = g2,
                                                 },
                                     },
                     };

        return medias.ToArray();
    }

    public ReactiveCommand<MediaFile, Unit> DownloadCommand { get; }

    public ReactiveCommand<Unit, Unit> GetMediasCommand { get; }

    [Reactive]
    public MediaDirectory[] Medias { get; set; } = Array.Empty<MediaDirectory>();
}
