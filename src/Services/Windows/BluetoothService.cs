using System;
using System.Threading.Tasks;
using DynamicData;
using GoProPilot.Models;

namespace GoProPilot.Services.Windows;

public class BluetoothService : IBluetoothService
{
    private readonly SourceCache<BluetoothDevice, string> _devices = new(_ => _.DeviceID);

    public BluetoothService()
    {
        Load();
    }

    public IObservable<IChangeSet<BluetoothDevice, string>> Connect() => _devices.Connect();

    private async void Load()
    {
        bool availability = false;

        //todo: set a timeout
        while (!availability)
        {
            availability = await InTheHand.Bluetooth.Bluetooth.GetAvailabilityAsync();
            await Task.Delay(500);
        }

        foreach (var d in await InTheHand.Bluetooth.Bluetooth.GetPairedDevicesAsync())
        {
            _devices.AddOrUpdate(new BluetoothDevice(d)
            {
                DeviceID = d.Id,
                Name = d.Name,
            });
        }
    }
}
