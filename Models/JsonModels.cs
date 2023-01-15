using System;
using System.Collections.Generic;
using GoProPilot.Converters;
using Newtonsoft.Json;

namespace GoProPilot.Models;

public class RawMediaDirectory
{
    [JsonProperty("d")]
    public string Directory { get; set; } = "";

    [JsonProperty("fs")]
    public IEnumerable<RawMediaFile> Files { get; set; } = Array.Empty<RawMediaFile>();
}

public class RawMediaFile
{
    [JsonProperty("cre")]
    [JsonConverter(typeof(JsonDateConverter))]
    public DateTime Created { get; set; }

    public long glrv { get; set; }

    // Low resolution video file size
    public long ls { get; set; }

    // Last modified time (seconds since epoch)
    [JsonProperty("mod")]
    [JsonConverter(typeof(JsonDateConverter))]
    public DateTime Modified { get; set; }

    [JsonProperty("n")]
    public string Name { get; set; } = "";

    // Size of (group) media in bytes
    [JsonProperty("s")]
    public long Size { get; set; }
}

public class RawMediaListResponse
{
    [JsonProperty("id")]
    public string Id { get; set; } = "";

    [JsonProperty("media")]
    public RawMediaDirectory[] Media { get; set; } = Array.Empty<RawMediaDirectory>();
}
