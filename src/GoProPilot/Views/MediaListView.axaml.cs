using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DryIoc;
using FluentAvalonia.UI.Controls;
using GoProPilot.ViewModels;

namespace GoProPilot.Views;

public partial class MediaListView : UserControl
{
    private readonly DownloadViewModel _downloadVM;

    public MediaListView()
    {
        InitializeComponent();
        DataContext = Core.Container.Resolve<MediaListViewModel>();
        _downloadVM = Core.Container.Resolve<DownloadViewModel>();
    }

    private void MediaFileHyperlink_Click(object sender, RoutedEventArgs e)
    {
        if (sender is HyperlinkButton btn)
            if (btn.Content != null && btn.Tag != null)
            {
                //_downloadVM.AddTask((string)btn.Content, (string)btn.Tag);

                //Console.WriteLine(btn.Tag);
                //Application.Current?.Clipboard?.SetTextAsync((string)btn.Tag);
            }
    }
}
