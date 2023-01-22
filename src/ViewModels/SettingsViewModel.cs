using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using GoProPilot.Models;
using GoProPilot.Services.Windows;
using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace GoProPilot.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private readonly ReadOnlyObservableCollection<BluetoothDeviceWrapper> _bluetoothDevices;
    private readonly ReadOnlyObservableCollection<WLANDeviceWrapper> _wlanDevices;
    private Config _config = new();
    private bool _isLoading;

    public SettingsViewModel()
    {
        LoadSettings();
        DownloadFolder = _config.DownloadFolder;

        var bthService = new BluetoothService();
        bthService.Connect()

            // Ensure the updates arrive on the UI thread.
            .ObserveOn(RxApp.MainThreadScheduler)

            // We .Bind() and now our mutable _bluetoothDevices collection
            // contains the new items and the GUI gets refreshed.
            .Bind(out _bluetoothDevices)
            .Subscribe();

        var wlanService = new WLANService();
        wlanService.Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _wlanDevices)
            .Subscribe();

        ((INotifyCollectionChanged)_bluetoothDevices).CollectionChanged += BluetoothDevices_Changed;
        ((INotifyCollectionChanged)_wlanDevices).CollectionChanged += WLANDevices_Changed;

        TestCommand = ReactiveCommand.Create(ExecuteTest);

        PropertyChanged += DoPropertyChanged;
    }

    public void LoadSettings()
    {
        _isLoading = true;
        try
        {
            if (File.Exists("Config.json"))
            {
                using var sr = new StreamReader("Config.json");
                var str = sr.ReadToEnd();
                var config = JsonConvert.DeserializeObject<Config>(str);
                if (config != null)
                {
                    _config = config;
                }
            }
        }
        finally
        {
            _isLoading = false;
        }
    }

    public void SaveSettings()
    {
        if (CurrentWLAN != null)
            _config.WLANDeviceID = CurrentWLAN.DeviceID;
        if (CurrentBluetooth != null)
            _config.CameraDeviceID = CurrentBluetooth.DeviceID;

        using var sw = new StreamWriter("Config.json");
        sw.Write(JsonConvert.SerializeObject(_config, Formatting.Indented));
        sw.Close();
    }

    private void BluetoothDevices_Changed(object? sender, NotifyCollectionChangedEventArgs e)
    {
        CurrentBluetooth = _bluetoothDevices.Where(_ => _.DeviceID == _config.CameraDeviceID).FirstOrDefault();
    }

    private void DoPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_isLoading) return;

        SaveSettings();
    }

    private void ExecuteTest()
    {
        if (_wlanDevices.Any())
        {
            CurrentWLAN = _wlanDevices.First();
        }
    }

    private void WLANDevices_Changed(object? sender, NotifyCollectionChangedEventArgs e)
    {
        CurrentWLAN = _wlanDevices.Where(_ => _.DeviceID == _config.WLANDeviceID).FirstOrDefault();
    }

    public ReadOnlyObservableCollection<BluetoothDeviceWrapper> BluetoothDevices => _bluetoothDevices;

    [Reactive]
    public BluetoothDeviceWrapper? CurrentBluetooth { get; set; }

    [Reactive]
    public WLANDeviceWrapper? CurrentWLAN { get; set; }

    [Reactive]
    public string DownloadFolder { get; set; } = "";

    public ReactiveCommand<Unit, Unit> TestCommand { get; }

    public ReadOnlyObservableCollection<WLANDeviceWrapper> WLANDevices => _wlanDevices;
}
