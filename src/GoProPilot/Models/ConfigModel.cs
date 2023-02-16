using Newtonsoft.Json;

namespace GoProPilot.Models;

public class Config
{
    [JsonProperty("ver")]
    public string Version { get; } = "1.0";

    [JsonProperty("camera")]
    public string? CameraDeviceID { get; set; }

    [JsonProperty("downloadFolder")]
    public string DownloadFolder { get; set; } = ".\\Download";

    [JsonProperty("wlan")]
    public string? WLANDeviceID { get; set; }
}
