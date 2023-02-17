using System;
using System.ComponentModel;
using System.Windows.Threading;
using DryIoc;
using GoProPilot.ViewModels;

namespace GoProPilot;

internal class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Core.IsDesignMode = LicenseManager.UsageMode == LicenseUsageMode.Designtime;
        Core.MainThreadInvokeAsync = async a => await Dispatcher.CurrentDispatcher.InvokeAsync(a);

        Globals.Init();

        App app = new App();
        app.InitializeComponent();
        app.Run();
    }
}
