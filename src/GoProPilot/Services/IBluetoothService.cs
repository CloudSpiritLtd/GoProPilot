using System;
using DynamicData;
using GoProPilot.Models;

namespace GoProPilot.Services;

public interface IBluetoothService
{
    IObservable<IChangeSet<BluetoothDeviceModel, string>> Connect();
}
