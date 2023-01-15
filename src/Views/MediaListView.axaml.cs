using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GoProPilot.Views;

public partial class MediaListView : UserControl
{
    public MediaListView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

}
