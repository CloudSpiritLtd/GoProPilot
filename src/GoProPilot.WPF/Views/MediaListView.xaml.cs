using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DryIoc;
using GoProPilot.Models;
using GoProPilot.ViewModels;

namespace GoProPilot.Views;

/// <summary>
/// MediaListView.xaml 的交互逻辑
/// </summary>
public partial class MediaListView : UserControl
{
    public MediaListView()
    {
        InitializeComponent();
        DownloadVM = Globals.Container.Resolve<DownloadViewModel>();
    }

    private void MediaFileHyperlink_Click(object sender, RoutedEventArgs e)
    {
        if (e.Source is Hyperlink hyperlink)
        {
            //DownloadVM.AddTask(((RawMediaFile)hyperlink.DataContext).Name, hyperlink.NavigateUri);
        }
    }
    public DownloadViewModel DownloadVM { get; }
}
