using System;
using Avalonia;
using Avalonia.ReactiveUI;
using DryIoc;
using FluentAvalonia.UI.Windowing;
using GoProPilot.ViewModels;
using Starlex.Avalonia.Converters;

namespace GoProPilot;

internal class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        Globals.Init();

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        GC.KeepAlive(typeof(FileSizeToStringConverter).Assembly);
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI()
            .With(new Win32PlatformOptions
            {
                //UseCompositor = false,      // Disable to make ClipToBounds work correctly. But will it affect something?
                UseWindowsUIComposition = true,
                CompositionBackdropCornerRadius = 8f,
            })
            .UseFAWindowing();
    }
}
