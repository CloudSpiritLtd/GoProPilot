using System.IO;
using GoProPilot.Models;
using Newtonsoft.Json;

namespace GoProPilot;

public class ConfigService
{
    private const string CONFIG_FILE = "Config.json";
    private Config _config = new();

    public void Load()
    {
        if (File.Exists(CONFIG_FILE))
        {
            using var sr = new StreamReader(CONFIG_FILE);
            var str = sr.ReadToEnd();
            var config = JsonConvert.DeserializeObject<Config>(str);
            if (config != null)
            {
                _config = config;
            }
        }
    }

    public void Save()
    {
        using var sw = new StreamWriter(CONFIG_FILE);
        sw.Write(JsonConvert.SerializeObject(_config, Formatting.Indented));
        sw.Close();
    }

    public Config Config { get => _config; }
}
