using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using DryIoc;
using FluentAvalonia.UI.Controls;
using GoProPilot.Services;
using GoProPilot.Services.Windows;
using ManagedNativeWifi;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace GoProPilot.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly SettingsViewModel _settingsVM;
    private readonly Dispatcher _dispatcher = Dispatcher.UIThread;

    public MainViewModel()
    {
        _settingsVM = Globals.Container.Resolve<SettingsViewModel>();
        _settingsVM.PropertyChanged += SettingsVM_PropertyChanged;

        ConnectCommand = ReactiveCommand.Create(ExecuteConnect);
        PropertyChanged += VM_PropertyChanged;

        if (_settingsVM.CurrentBluetooth != null)
            SetupCamera();
        BuildConnectButtonText();
    }

    private void BuildConnectButtonText()
    {
        if (_settingsVM.CurrentBluetooth == null)
            ConnectButtonText = "No camera selected";
        else
            ConnectButtonText = $"{(IsConnected ? "Disconnect" : "Connect")} camera {_settingsVM.CurrentBluetooth.Name}";
    }

    private void Camera_APStateChanged(object? sender, bool e)
    {
        _dispatcher.InvokeAsync(() =>
        {
            WifiAPState = e ? State.Active : State.Inactive;
        });
    }

    private async Task ConnectBluetoothAsync()
    {
        if (CameraState == State.Active || Camera == null)
            return;

        await Camera.ConnectAsync();
        if (Camera.IsConnected)
        {
            await Camera.SetupAsync();
        }

        CameraState = Camera.IsConnected ? State.Active : State.Inactive;
    }

    private async Task ConnectWiFiAsync()
    {
        if (CameraState != State.Active)
        {
            await Utils.ShowErrorMessageAsync("Camera not connected");
            return;
        }

        var timeout = TimeSpan.FromSeconds(30);
        await Camera!.TogglefWifiAP(true);

        // Check if connected to GoPro AP
        if (_settingsVM.CurrentWLAN!.RawDevice.State == InterfaceState.Connected)
        {
            if (NativeWifi.EnumerateConnectedNetworkSsids().Any(nid => nid.ToString() == Camera!.WifiSSID))
            {
                WifiState = State.Active;
                return;
            }
            else
            {
                // Disconnect first, if not connect to the camera
                var disconnected = await NativeWifi.DisconnectNetworkAsync(_settingsVM.CurrentWLAN.RawDevice.Id, timeout);
                if (!disconnected)
                    throw new Exception("Selected WLAN Interface is busy. Choose another one or manually disconnect it.");
            }
        }

        // Check profile
        if (!NativeWifi.EnumerateProfileNames().Contains(Camera!.WifiSSID))
        {
            var profileXml = string.Format(GoProPilot.Resources.WIFI_PROFILE_TEMPLATE, Camera!.WifiSSID, Camera!.WifiPassword);
            if (!NativeWifi.SetProfile(_settingsVM.CurrentWLAN.RawDevice.Id, ProfileType.AllUser, profileXml, null, true))
            {
                throw new Exception("Cannot set Wifi Profile.");
            }
        }

        // Connect
        var counter = 0;
        var connected = false;
        do
        {
            connected = await NativeWifi.ConnectNetworkAsync(_settingsVM.CurrentWLAN.RawDevice.Id, Camera!.WifiSSID, BssType.Infrastructure, timeout);
            counter++;
        } while (counter <= 3 && !connected);
        WifiState = connected ? State.Active : State.Inactive;
    }

    private async void DisconnectAsync()
    {
        if (WifiState != State.Active || _settingsVM.CurrentWLAN == null)
            return;

        var timeout = TimeSpan.FromSeconds(30);
        if (await NativeWifi.DisconnectNetworkAsync(_settingsVM.CurrentWLAN.RawDevice.Id, timeout))
            WifiState = State.Inactive;
        else
            WifiState = State.Error;

        if (CameraState != State.Active || Camera == null)
            return;

        Camera.Disconnect();
        CameraState = State.Inactive;
        WifiAPState = State.Inactive;
    }

    private async void ExecuteConnect()
    {
        if (_settingsVM.CurrentBluetooth == null || _settingsVM.CurrentWLAN == null)
        {
            //todo: use INavigationService, auto nav to settings page.
            await Utils.ShowErrorMessageAsync("You need to select proper devices in Settings page first.");
            return;
        }

        if (IsConnected)
        {
            DisconnectAsync();
        }
        else
        {
            await ConnectBluetoothAsync();
            await ConnectWiFiAsync();
        }
    }

    private void SettingsVM_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SettingsViewModel.CurrentBluetooth))
        {
            if (_settingsVM.CurrentBluetooth == null)
            {
                Camera = null;
            }
            else
            {
                SetupCamera();
                BuildConnectButtonText();
            }
        }
    }

    private void SetupCamera()
    {
        Camera = new CameraService(_settingsVM.CurrentBluetooth!.RawDevice);
        Camera.APStateChanged += Camera_APStateChanged;
    }

    private void VM_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CameraState) || e.PropertyName == nameof(WifiAPState) || e.PropertyName == nameof(WifiState))
        {
            IsConnected = CameraState == State.Active && WifiAPState == State.Active && WifiState == State.Active;
            BuildConnectButtonText();
        }
    }

    public ICameraService? Camera { get; private set; }

    [Reactive]
    public State CameraState { get; set; }

    [Reactive]
    public string ConnectButtonText { get; private set; } = "No Camera";

    public ReactiveCommand<Unit, Unit> ConnectCommand { get; private set; }

    [Reactive]
    public bool IsConnected { get; private set; }

    [Reactive]
    public bool ShowToast { get; set; }

    [Reactive]
    public string Toast { get; set; } = "";

    [Reactive]
    public State WifiAPState { get; set; }

    [Reactive]
    public State WifiState { get; set; }
}
