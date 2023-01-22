using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DryIoc;
using FluentAvalonia.UI.Controls;
using GoProPilot.Services;
using GoProPilot.ViewModels;

namespace GoProPilot.Views;

public partial class MainView : UserControl, INavigationService
{
    private Frame? _frameView;
    private NavigationView? _navView;

    public MainView()
    {
        InitializeComponent();

        DataContext = Globals.Container.Resolve<MainViewModel>();
        Globals.Container.RegisterInstance<INavigationService>(this, IfAlreadyRegistered.Replace);
    }

    public void NavigateTo(Type type)
    {
        _frameView?.Navigate(typeof(HomeView));
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        _frameView = this.FindControl<Frame>("FrameView");
        _navView = this.FindControl<NavigationView>("NavView");
        if (_navView != null)
        {
            //_navView.MenuItems = GetNavigationViewItems();
            //_navView.FooterMenuItems = GetFooterNavigationViewItems();
            _navView.ItemInvoked += OnNavigationViewItemInvoked;

            _navView.SelectedItem = _navView.MenuItems.Cast<NavigationViewItem>().FirstOrDefault();
            _frameView?.Navigate(typeof(HomeView));
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnNavigationViewItemInvoked(object? sender, NavigationViewItemInvokedEventArgs e)
    {
        // Change the current selected item back to normal
        //SetNVIIcon(_navView.SelectedItem as NavigationViewItem, false);

        if (e.InvokedItemContainer is NavigationViewItem nvi && nvi.Tag is Type t)
        {
            _frameView?.Navigate(t, null, e.RecommendedNavigationTransitionInfo);
        }
    }
}
