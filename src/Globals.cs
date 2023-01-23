using System.Runtime.InteropServices;
using DryIoc;
using GoProPilot.Services;
using GoProPilot.ViewModels;

namespace GoProPilot;

public static class Globals
{
    public static readonly IContainer Container;

    static Globals()
    {
        Container = new Container();
        Container.Register<IBluetoothService, Services.Windows.BluetoothService>(Reuse.Singleton, serviceKey: OSPlatform.Windows);
        Container.Register<IWLANService, Services.Windows.WLANService>(Reuse.Singleton, serviceKey: OSPlatform.Windows);
        Container.Register<ICameraService, Services.Windows.CameraService>(Reuse.Singleton, serviceKey: OSPlatform.Windows);

        Container.Register<MainViewModel>(Reuse.Singleton);
        Container.Register<SettingsViewModel>(Reuse.Singleton);
        Container.Register<MediaListViewModel>(Reuse.Singleton);
    }

    public static void Init()
    {
    }
}
