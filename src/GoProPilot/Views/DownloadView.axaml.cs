using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DryIoc;
using GoProPilot.ViewModels;

namespace GoProPilot.Views
{
    public partial class DownloadView : UserControl
    {
        public DownloadView()
        {
            InitializeComponent();
            DataContext = Core.Container.Resolve<DownloadViewModel>();
        }
    }
}
