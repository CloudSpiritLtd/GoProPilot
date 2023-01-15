using ManagedNativeWifi;

namespace GoProPilot.Models;

public abstract class BaseDevice<T>
    where T : class
{
    public BaseDevice(T device)
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

public class BluetoothDevice : BaseDevice<InTheHand.Bluetooth.BluetoothDevice>
{
    public BluetoothDevice(InTheHand.Bluetooth.BluetoothDevice device) : base(device)
    {
    }
}

public class WLANDevice : BaseDevice<InterfaceInfo>
{
    public WLANDevice(InterfaceInfo device) : base(device)
    {
    }
}
