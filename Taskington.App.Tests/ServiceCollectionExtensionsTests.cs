using Microsoft.Extensions.DependencyInjection;
using Taskington.App.ViewModels;
using Xunit;

namespace Taskington.App.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddTaskingtonAppReturnsTheSameCollection()
    {
        var services = new ServiceCollection();

        var result = services.AddTaskingtonApp();

        Assert.Same(services, result);
    }

    [Fact]
    public void AddTaskingtonAppResolvesTheMainWindowViewModel()
    {
        var serviceProvider = new ServiceCollection()
            .AddTaskingtonApp()
            .BuildServiceProvider();

        var viewModel = serviceProvider.GetRequiredService<MainWindowViewModel>();

        Assert.IsType<MainWindowViewModel>(viewModel);
    }

    [Fact]
    public void AddTaskingtonAppCreatesNewViewModelInstancePerWindow()
    {
        var serviceProvider = new ServiceCollection()
            .AddTaskingtonApp()
            .BuildServiceProvider();

        var first = serviceProvider.GetRequiredService<MainWindowViewModel>();
        var second = serviceProvider.GetRequiredService<MainWindowViewModel>();

        Assert.NotSame(first, second);
    }
}
