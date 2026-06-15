using Avalonia;
using ReactiveUI.Avalonia;

namespace Taskington.Gui;

static class Program
{
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    private static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI(_ => { })
            .RegisterReactiveUIViewsFromEntryAssembly();
    }
}