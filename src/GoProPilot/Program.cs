using System;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using DryIoc;
using FluentAvalonia.UI.Windowing;
using GoProPilot.ViewModels;
using ReactiveUI;
using Starlex.Avalonia.Converters;

namespace GoProPilot;

internal class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static async Task Main(string[] args)
    {
        Core.IsDesignMode = Design.IsDesignMode;
        Core.MainThreadInvokeAsync = a => Dispatcher.UIThread.InvokeAsync(a);

        Globals.Init();

        RxApp.DefaultExceptionHandler = Observer.Create<Exception>(async ex =>
        {
            await Utils.ShowErrorMessageAsync(ex.Message);
        });

        try
        {
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            await Utils.ShowErrorMessageAsync(ex.Message);
        }
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
