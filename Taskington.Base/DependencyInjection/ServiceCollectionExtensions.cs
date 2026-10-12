using Microsoft.Extensions.DependencyInjection;
using Taskington.Base.FileSystem;
using Taskington.Base.Logging;
using Taskington.Base.Platform;

namespace Taskington.Base.DependencyInjection;

/// <summary>
/// DI registration for the Taskington.Base module: platform directories, file
/// system abstraction and logging bootstrap. App-level modules call this from
/// their own registration extension (spec chapter 3).
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTaskingtonBase(this IServiceCollection services)
    {
        services.AddSingleton<IFileSystemService, FileSystemService>();
        services.AddSingleton<PlatformDirectoryService>();
        services.AddSingleton<LoggingBootstrap>();
        return services;
    }
}
