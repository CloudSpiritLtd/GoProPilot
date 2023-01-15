using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using GoProPilot.Models;
using Newtonsoft.Json;

namespace GoProPilot.Services;

public class WebApi
{
    public static readonly WebApi Instance = new();

    private readonly HttpClient _httpClient = new();
    private readonly Timer _keepAliveTimer;

    private WebApi()
    {
        _keepAliveTimer = new Timer(3000);
        _keepAliveTimer.Elapsed += (_, _) =>
        {
            _httpClient.GetAsync("http://10.5.5.9:8080/gopro/camera/keep_alive");
        };
    }

    public async Task DeleteMediaFiles(string dir, IEnumerable<string> filenames)
    {
        foreach (var file in filenames)
        {
            var url = $"http://10.5.5.9:8080/gopro/media/delete/file?path={dir}/{file}";
            await _httpClient.GetAsync(url);
        }
    }

    public async Task<MediaDirectory[]> GetMediaGroupsAsync()
    {
        var res = await _httpClient.GetAsync("http://10.5.5.9:8080/gopro/media/list");
        res.EnsureSuccessStatusCode();
        var str = await res.Content.ReadAsStringAsync();
        var json = JsonConvert.DeserializeObject<RawMediaListResponse>(str);
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
                                                     Files = g2,
                                                 },
                                     },
                     };
        return medias.ToArray();
    }

    public void StartKeepAlive()
    {
        _keepAliveTimer.Start();
    }

    public void StopKeepAlive()
    {
        _keepAliveTimer.Stop();
    }
}
