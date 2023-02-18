using System;
using System.Windows;
using System.Windows.Controls;
using DryIoc;
using GoProPilot.ViewModels;

namespace GoProPilot.Views;

/// <summary>
/// MainView.xaml 的交互逻辑
/// </summary>
public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        DataContext = Globals.Container.Resolve<MainViewModel>();
    }

    private void SetupBtnClick(object sender, RoutedEventArgs e)
    {
        var dialog = new SetupDialog();
        dialog.ShowDialog();

        //dialog.ViewModel.SelectedWLANInterfaceId;
    }
}
