using InTheHand.Bluetooth;
using ManagedNativeWifi;

namespace GoProPilot.Models;

public abstract class BaseDeviceModel<T>
    where T : class
{
    public BaseDeviceModel(T device)
    {
        RawDevice = device;
    }

    public override string ToString()
    {
        return Name;
    }

    public string DeviceID { get; init; } = "";

    public string Name { get; init; } = "";

    public T RawDevice { get; init; }
}

public class BluetoothDeviceModel : BaseDeviceModel<BluetoothDevice>
{
    public BluetoothDeviceModel(BluetoothDevice device) : base(device)
    {
        DeviceID = device.Id;
        Name = device.Name;
    }
}

public class WLANDeviceModel : BaseDeviceModel<InterfaceInfo>
{
    public WLANDeviceModel(InterfaceInfo device) : base(device)
    {
        DeviceID = device.Id.ToString();
        Name = device.Description;
    }
}
