using Newtonsoft.Json;

namespace GoProPilot.Models;

public class Config
{
    [JsonProperty("camera")]
    public string CameraDeviceID { get; set; } = "";

    [JsonProperty("downloadFolder")]
    public string DownloadFolder { get; set; } = "";

    [JsonProperty("wlan")]
    public string WLANDeviceID { get; set; } = "";
}
