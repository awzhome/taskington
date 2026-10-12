using Avalonia;
using Avalonia.ReactiveUI;
using Serilog;
using System;
using Taskington.Base.Logging;
using Taskington.Base.Platform;

namespace Taskington.App;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        var platformDirectories = new PlatformDirectoryService();
        platformDirectories.EnsureDirectories();
        LoggingBootstrap.Configure(platformDirectories, args);

        try
        {
            Log.Information("Taskington starting");
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        finally
        {
            LoggingBootstrap.ShutDown();
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}