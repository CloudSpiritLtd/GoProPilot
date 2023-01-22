using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DryIoc;
using GoProPilot.ViewModels;

namespace GoProPilot.Views
{
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();

            DataContext = Globals.Container.Resolve<SettingsViewModel>();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

    }
}
