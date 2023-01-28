using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace GoProPilot.Models;

public class Config : ReactiveObject
{
    [JsonProperty("camera")]
    [Reactive]
    public string CameraDeviceID { get; set; } = "";

    [JsonProperty("downloadFolder")]
    [Reactive]
    public string DownloadFolder { get; set; } = "";

    [JsonProperty("wlan")]
    [Reactive]
    public string WLANDeviceID { get; set; } = "";
}
