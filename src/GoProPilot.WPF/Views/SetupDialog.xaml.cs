using System;
using System.Collections.ObjectModel;
using System.Windows;
using GoProPilot.ViewModels;
using ManagedNativeWifi;
//using Prism.Mvvm;

namespace GoProPilot;

/// <summary>
/// SetupDialog.xaml 的交互逻辑
/// </summary>
public partial class SetupDialog : Window
{
    public SetupDialog()
    {
        InitializeComponent();

        DataContext = ViewModel;
    }

    public SetupDialogViewModel ViewModel { get; } = new();
}

public class SetupDialogViewModel : ViewModelBase
{
    public SetupDialogViewModel()
    {
        foreach (var i in NativeWifi.EnumerateInterfaces())
        {
            Interfaces.Add(i);
        }
    }

    public ObservableCollection<InterfaceInfo> Interfaces { get; } = new();

    public Guid? SelectedWLANInterfaceId { get; set; }
}
