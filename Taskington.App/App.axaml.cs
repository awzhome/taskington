using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Taskington.App.Views;

namespace Taskington.App;

public class App : Application
{
    private readonly IServiceProvider? services;

    /// <summary>
    /// Design-time constructor, used by the visual designer via BuildAvaloniaApp().
    /// </summary>
    public App()
    {
    }

    public App(IServiceProvider services)
    {
        this.services = services;
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (services is not null
            && ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = services.GetRequiredService<MainWindow>();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
