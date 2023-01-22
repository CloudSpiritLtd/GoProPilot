using System;
using System.Threading.Tasks;
using DynamicData;
using GoProPilot.Models;
using InTheHand.Bluetooth;

namespace GoProPilot.Services.Windows;

// For Windows, macOS
public class BluetoothService : IBluetoothService
{
    private readonly SourceCache<BluetoothDeviceWrapper, string> _devices = new(_ => _.DeviceID);

    public BluetoothService()
    {
        Load();
    }

    public IObservable<IChangeSet<BluetoothDeviceWrapper, string>> Connect() => _devices.Connect();

    private async void Load()
    {
        bool availability = false;

        //todo: set a timeout
        while (!availability)
        {
            availability = await Bluetooth.GetAvailabilityAsync();
            await Task.Delay(500);
        }

        foreach (var d in await Bluetooth.GetPairedDevicesAsync())
        {
            _devices.AddOrUpdate(new BluetoothDeviceWrapper(d));
        }
    }
}
