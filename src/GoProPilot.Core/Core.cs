using System.Runtime.InteropServices;
using DryIoc;
using GoProPilot.Services;
using GoProPilot.ViewModels;

namespace GoProPilot;

public static class Core
{
    public static readonly IContainer Container;

    static Core()
    {
        Container = new Container();
        Container.Register<ConfigService>(Reuse.Singleton);
        Container.Register<DownloadService>(Reuse.Singleton);
        Container.Register<IBluetoothService, Services.Windows.BluetoothService>(Reuse.Singleton, serviceKey: OSPlatform.Windows);
        Container.Register<IWLANService, Services.Windows.WLANService>(Reuse.Singleton, serviceKey: OSPlatform.Windows);
        Container.Register<ICameraService, Services.Windows.CameraService>(Reuse.Singleton, serviceKey: OSPlatform.Windows);

        Container.Register<MainViewModel>(Reuse.Singleton);
        Container.Register<SettingsViewModel>(Reuse.Singleton);
        Container.Register<MediaListViewModel>(Reuse.Singleton);
        Container.Register<DownloadViewModel>(Reuse.Singleton);

    }

    public static bool IsDesignMode { get; set; }

    public static Func<Action, Task> MainThreadInvokeAsync { get; set; } = delegate { return Task.Delay(0); };
}
