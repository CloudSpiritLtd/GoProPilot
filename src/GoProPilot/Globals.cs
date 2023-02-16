using System.Runtime.InteropServices;
using DryIoc;
using GoProPilot.Services;
using GoProPilot.ViewModels;

namespace GoProPilot;

public static class Globals
{
    static Globals()
    {
        Core.Container.Register<MainViewModel>(Reuse.Singleton);
        Core.Container.Register<SettingsViewModel>(Reuse.Singleton);
        Core.Container.Register<MediaListViewModel>(Reuse.Singleton);
        Core.Container.Register<DownloadViewModel>(Reuse.Singleton);
    }

    public static void Init()
    {
        var cfgSvc = Core.Container.Resolve<ConfigService>();
        cfgSvc.Load();

        //todo: detect platform
        Core.Container.Resolve<IBluetoothService>(OSPlatform.Windows);
        Core.Container.Resolve<IWLANService>(OSPlatform.Windows);
    }
}
