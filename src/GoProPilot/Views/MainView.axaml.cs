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
    public MainView()
    {
        InitializeComponent();
        NavView.ItemInvoked += OnNavigationViewItemInvoked;

        DataContext = Core.Container.Resolve<MainViewModel>();
        Core.Container.RegisterInstance<INavigationService>(this, IfAlreadyRegistered.Replace);
    }

    public void NavigateTo(Type type)
    {
        FrameView.Navigate(type);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        //_frameView = this.FindControl<Frame>("FrameView");
        //_navView = this.FindControl<NavigationView>("NavView");

        NavView.SelectedItem = NavView.MenuItems.Cast<NavigationViewItem>().FirstOrDefault();
        FrameView.Navigate(typeof(HomeView));
    }

    private void OnNavigationViewItemInvoked(object? sender, NavigationViewItemInvokedEventArgs e)
    {
        // Change the current selected item back to normal
        //SetNVIIcon(_navView.SelectedItem as NavigationViewItem, false);

        if (e.InvokedItemContainer is NavigationViewItem nvi && nvi.Tag is Type t)
        {
            FrameView.Navigate(t, null, e.RecommendedNavigationTransitionInfo);
        }
    }
}
