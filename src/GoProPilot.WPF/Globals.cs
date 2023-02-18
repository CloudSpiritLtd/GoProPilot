using System.Runtime.InteropServices;
using DryIoc;
using GoProPilot.Services;
using GoProPilot.ViewModels;

namespace GoProPilot;

public static class Globals
{
    public static readonly IContainer Container = Core.Container;

    static Globals()
    {
    }

    public static void Init()
    {
        var cfgSvc = Container.Resolve<ConfigService>();
        cfgSvc.Load();

        //todo: detect platform
        Container.Resolve<IBluetoothService>(OSPlatform.Windows);
        Container.Resolve<IWLANService>(OSPlatform.Windows);
    }

    public static NavigationViewModel NavigationVM { get; } = new NavigationViewModel();

}
