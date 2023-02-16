using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using DryIoc;
using DynamicData;
using GoProPilot.Models;
using GoProPilot.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace GoProPilot.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private readonly ReadOnlyObservableCollection<BluetoothDeviceModel> _bluetoothDevices;
    private readonly Config _config;
    private readonly ConfigService _configService;
    private readonly ReadOnlyObservableCollection<WLANDeviceModel> _wlanDevices;

    public SettingsViewModel()
    {
        _configService = Core.Container.Resolve<ConfigService>();
        _config = _configService.Config;
        DownloadFolder = _config.DownloadFolder;

        var bthService = Core.Container.Resolve<IBluetoothService>(OSPlatform.Windows);
        bthService.Connect()

            // Ensure the updates arrive on the UI thread.
            .ObserveOn(RxApp.MainThreadScheduler)

            // We .Bind() and now our mutable _bluetoothDevices collection
            // contains the new items and the GUI gets refreshed.
            .Bind(out _bluetoothDevices)
            .Subscribe(_ =>
            {
                CurrentBluetooth = _bluetoothDevices.Where(_ => _.DeviceID == _config.CameraDeviceID).FirstOrDefault();
            });

        var wlanService = Core.Container.Resolve<IWLANService>(OSPlatform.Windows);
        wlanService.Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _wlanDevices)
            .Subscribe(_ =>
            {
                CurrentWLAN = _wlanDevices.Where(_ => _.DeviceID == _config.WLANDeviceID).FirstOrDefault();
            });

        TestCommand = ReactiveCommand.Create(ExecuteTest);

        /*
        this.WhenAnyValue(_ => _.CurrentBluetooth, _ => _.CurrentWLAN, _ => _.DownloadFolder)
            .Subscribe(a =>
            {
                if (!_loading)
                {
                    _config.CameraDeviceID = a.Item1?.DeviceID;
                    _config.WLANDeviceID = a.Item2?.DeviceID;
                    _config.DownloadFolder = a.Item3;
                }
            });
        */

        this.WhenAnyValue(_ => _.CurrentBluetooth)
            .Subscribe(a => _config.CameraDeviceID = a?.DeviceID);

        this.WhenAnyValue(_ => _.CurrentWLAN)
            .Subscribe(a => _config.WLANDeviceID = a?.DeviceID);

        this.WhenAnyValue(_ => _.DownloadFolder)
            .Subscribe(a => _config.DownloadFolder = a);
    }

    private void ExecuteTest()
    {
        _configService.Save();
    }

    public ReadOnlyObservableCollection<BluetoothDeviceModel> BluetoothDevices => _bluetoothDevices;

    [Reactive]
    public BluetoothDeviceModel? CurrentBluetooth { get; set; }

    [Reactive]
    public WLANDeviceModel? CurrentWLAN { get; set; }

    [Reactive]
    public string DownloadFolder { get; set; }

    public ReactiveCommand<Unit, Unit> TestCommand { get; }

    public ReadOnlyObservableCollection<WLANDeviceModel> WLANDevices => _wlanDevices;
}
