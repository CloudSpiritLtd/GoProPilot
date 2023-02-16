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

    public async Task<string> GetMediaGroupsAsync()
    {
        var res = await _httpClient.GetAsync("http://10.5.5.9:8080/gopro/media/list");
        res.EnsureSuccessStatusCode();
        var str = await res.Content.ReadAsStringAsync();
        return str;
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
