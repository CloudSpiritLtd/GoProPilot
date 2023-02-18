using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using GoProPilot.ViewModels;

namespace GoProPilot.Views
{
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
}
