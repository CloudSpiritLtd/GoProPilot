using System;
using DynamicData;
using GoProPilot.Models;

namespace GoProPilot.Services;

public interface IBluetoothService
{
    IObservable<IChangeSet<BluetoothDeviceWrapper, string>> Connect();
}
