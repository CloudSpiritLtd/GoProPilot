using InTheHand.Bluetooth;
using ManagedNativeWifi;

namespace GoProPilot.Models;

public abstract class BaseDeviceWrapper<T>
    where T : class
{
    public BaseDeviceWrapper(T device)
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

public class BluetoothDeviceWrapper : BaseDeviceWrapper<BluetoothDevice>
{
    public BluetoothDeviceWrapper(BluetoothDevice device) : base(device)
    {
        DeviceID = device.Id;
        Name = device.Name;
    }
}

public class WLANDeviceWrapper : BaseDeviceWrapper<InterfaceInfo>
{
    public WLANDeviceWrapper(InterfaceInfo device) : base(device)
    {
        DeviceID = device.Id.ToString();
        Name = device.Description;
    }
}
