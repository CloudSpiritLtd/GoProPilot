using Avalonia.Controls;
using DryIoc;
using GoProPilot.ViewModels;

namespace GoProPilot.Views
{
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();

            DataContext = Core.Container.Resolve<SettingsViewModel>();
        }
    }
}
