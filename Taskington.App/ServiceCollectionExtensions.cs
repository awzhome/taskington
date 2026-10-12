using Microsoft.Extensions.DependencyInjection;
using Taskington.App.ViewModels;
using Taskington.App.Views;

namespace Taskington.App;

/// <summary>
/// DI registration for the Taskington.App module: ViewModels and views.
/// ViewModels and views are always resolved from the container, never manually
/// constructed (spec chapter 3, App AGENTS.md).
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTaskingtonApp(this IServiceCollection services)
    {
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient(serviceProvider => new MainWindow
        {
            DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>()
        });
        return services;
    }
}
