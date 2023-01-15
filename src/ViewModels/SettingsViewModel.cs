using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using GoProPilot.Models;
using GoProPilot.Services.Windows;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace GoProPilot.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private readonly ReadOnlyObservableCollection<BluetoothDevice> _bluetoothDevices;
    private readonly ReadOnlyObservableCollection<WLANDevice> _wlanDevices;

    public SettingsViewModel()
    {
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

        TestCommand = ReactiveCommand.Create(ExecuteTest);
    }

    private void ExecuteTest()
    {
        if (_wlanDevices.Any())
        {
            CurrentWLAN = _wlanDevices.First();
        }
    }

    //todo: load settings

    [Reactive]
    public WLANDevice? CurrentWLAN { get; set; }

    [Reactive]
    public BluetoothDevice? CurrentBluetooth { get; set; }

    public ReadOnlyObservableCollection<BluetoothDevice> BluetoothDevices => _bluetoothDevices;
    public ReadOnlyObservableCollection<WLANDevice> WLANDevices => _wlanDevices;

    public ReactiveCommand<Unit, Unit> TestCommand { get; }
}
