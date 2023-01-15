using System;
using System.Linq;
using System.Windows;
using DynamicData;
using GoProPilot.Models;
using ManagedNativeWifi;

namespace GoProPilot.Services.Windows;

public class WLANService : IWLANService
{
    private readonly SourceCache<WLANDevice, string> _devices = new(_ => _.DeviceID);

    public WLANService()
    {
        Load();
    }

    public IObservable<IChangeSet<WLANDevice, string>> Connect() => _devices.Connect();

    private void Load()
    {
        var intfs = NativeWifi.EnumerateInterfaces();
        if (!intfs.Any())
        {
            //todo: change to platform-independent
            //MessageBox.Show("No WLAN device found. You should have at least one. App will now exit.");
            //Application.Current.Shutdown();
        }

        foreach (var d in intfs)
        {
            _devices.AddOrUpdate(new WLANDevice(d)
            {
                DeviceID = d.Id.ToString(),
                Name = d.Description,
            });
        }
    }
}
