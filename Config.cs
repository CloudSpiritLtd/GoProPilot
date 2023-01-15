using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ManagedNativeWifi;
using Newtonsoft.Json;

namespace GoProPilot;

public class Config
{
    public string DownloadFolder { get; set; } = @"\\nas\video2\GoPro";
    public string WLANDeviceID { get; set; } = "";
}

public class RuntimeConfig
{
    public static RuntimeConfig Instance { get; } = new();

    private RuntimeConfig()
    {
        var intfs = NativeWifi.EnumerateInterfaces();
        if (!intfs.Any())
        {
            MessageBox.Show("No WLAN device found. You should have at least one. App will now exit.");
            Application.Current.Shutdown();
        }

        if (intfs.Count() == 1)
        {
            WLANInterface = intfs.First();
        }
    }

    public InterfaceInfo? WLANInterface { get; set; }

    public string DownloadFolder { get; set; } = @"\\nas\video2\GoPro";
}
