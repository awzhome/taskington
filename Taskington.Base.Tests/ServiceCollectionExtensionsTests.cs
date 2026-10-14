using Microsoft.Extensions.DependencyInjection;
using Taskington.Base.DependencyInjection;
using Taskington.Base.FileSystem;
using Taskington.Base.Logging;
using Taskington.Base.Platform;
using Taskington.Base.Scripting;
using Taskington.Base.Storage;
using Xunit;

namespace Taskington.Base.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddTaskingtonBaseReturnsTheSameCollection()
    {
        var services = new ServiceCollection();

        var result = services.AddTaskingtonBase();

        Assert.Same(services, result);
    }

    [Fact]
    public void AddTaskingtonBaseResolvesAllRootServices()
    {
        var serviceProvider = new ServiceCollection()
            .AddTaskingtonBase()
            .BuildServiceProvider();

        Assert.IsType<FileSystemService>(serviceProvider.GetRequiredService<IFileSystemService>());
        Assert.NotNull(serviceProvider.GetRequiredService<PlatformDirectoryService>());
        Assert.NotNull(serviceProvider.GetRequiredService<LoggingBootstrap>());
        Assert.NotNull(serviceProvider.GetRequiredService<JobRepository>());
        Assert.NotNull(serviceProvider.GetRequiredService<JobHistoryService>());
        Assert.NotNull(serviceProvider.GetRequiredService<LuaScriptEngine>());
        Assert.NotNull(serviceProvider.GetRequiredService<ScriptSandbox>());
    }

    [Fact]
    public void AddTaskingtonBaseRegistersServicesAsSingletons()
    {
        var serviceProvider = new ServiceCollection()
            .AddTaskingtonBase()
            .BuildServiceProvider();

        Assert.Same(
            serviceProvider.GetRequiredService<PlatformDirectoryService>(),
            serviceProvider.GetRequiredService<PlatformDirectoryService>());
        Assert.Same(
            serviceProvider.GetRequiredService<IFileSystemService>(),
            serviceProvider.GetRequiredService<IFileSystemService>());
        Assert.Same(
            serviceProvider.GetRequiredService<LoggingBootstrap>(),
            serviceProvider.GetRequiredService<LoggingBootstrap>());
        Assert.Same(
            serviceProvider.GetRequiredService<JobRepository>(),
            serviceProvider.GetRequiredService<JobRepository>());
        Assert.Same(
            serviceProvider.GetRequiredService<JobHistoryService>(),
            serviceProvider.GetRequiredService<JobHistoryService>());
        Assert.Same(
            serviceProvider.GetRequiredService<LuaScriptEngine>(),
            serviceProvider.GetRequiredService<LuaScriptEngine>());
        Assert.Same(
            serviceProvider.GetRequiredService<ScriptSandbox>(),
            serviceProvider.GetRequiredService<ScriptSandbox>());
    }
}
