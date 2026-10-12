using Avalonia;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using Taskington.Base.DependencyInjection;
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
        var serviceProvider = BuildContainer();

        var platformDirectories = serviceProvider.GetRequiredService<PlatformDirectoryService>();
        platformDirectories.EnsureDirectories();
        var loggingBootstrap = serviceProvider.GetRequiredService<LoggingBootstrap>();
        loggingBootstrap.Configure(platformDirectories, args);

        try
        {
            Log.Information("Taskington starting");
            BuildAvaloniaApp(new App(serviceProvider)).StartWithClassicDesktopLifetime(args);
        }
        finally
        {
            loggingBootstrap.ShutDown();
        }
    }

    private static ServiceProvider BuildContainer()
    {
        var services = new ServiceCollection();
        services.AddTaskingtonBase();
        services.AddTaskingtonApp();
        return services.BuildServiceProvider();
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => BuildAvaloniaApp(null);

    private static AppBuilder BuildAvaloniaApp(App? app)
        => (app is null ? AppBuilder.Configure<App>() : AppBuilder.Configure(() => app))
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}
