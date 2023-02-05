using System;
using System.Threading.Tasks;

namespace GoProPilot.Services;

public interface ICameraService
{
    Task ConnectAsync();

    void Disconnect();

    Task SetupAsync();

    Task TogglefWifiAP(bool turnOn);

    Task ToggleShutter(bool turnOn);

    bool IsConnected { get; }

    string? WifiPassword { get; }

    string? WifiSSID { get; }

    event EventHandler<bool> APStateChanged;
}
