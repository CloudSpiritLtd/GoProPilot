using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using DryIoc;
using FluentAvalonia.UI.Controls;
using GoProPilot.Models;
using GoProPilot.ViewModels;

namespace GoProPilot.Views;

public partial class MediaListView : UserControl
{
    public MediaListView()
    {
        InitializeComponent();
        DataContext = Globals.Container.Resolve<MediaListViewModel>();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void MediaFileHyperlink_Click(object sender, RoutedEventArgs e)
    {
        if (e.Source is HyperlinkButton btn)
        {
            //DownloadVM.AddTask(((RawMediaFile)hyperlink.DataContext).Name, hyperlink.NavigateUri);
            Console.WriteLine(btn.Tag);
        }
    }
}
