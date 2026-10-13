using System;
using System.IO;

namespace Taskington.Base.Platform;

/// <summary>
/// The desktop operating system Taskington is running on.
/// </summary>
public enum DesktopPlatform
{
    MacOS,
    Linux,
    Windows
}

/// <summary>
/// Resolves and creates the per-platform user configuration directory and its
/// subdirectories (see spec chapter 8): macOS ~/Library/Application Support/Taskington/,
/// Linux $XDG_CONFIG_HOME/taskington/ (default ~/.config/taskington/),
/// Windows %APPDATA%\Taskington\, each containing jobs/, history/ and logs/.
/// </summary>
public sealed class PlatformDirectoryService
{
    public DesktopPlatform Platform { get; }
    public string ConfigDirectory { get; }
    public string JobsDirectory { get; }
    public string HistoryDirectory { get; }
    public string LogsDirectory { get; }

    public PlatformDirectoryService()
        : this(DetectPlatform(), Environment.GetEnvironmentVariable, GetUserHomeDirectory())
    {
    }

    internal PlatformDirectoryService(
        DesktopPlatform platform,
        Func<string, string?> getEnvironmentVariable,
        string userHomeDirectory)
    {
        Platform = platform;
        ConfigDirectory = ResolveConfigDirectory(platform, getEnvironmentVariable, userHomeDirectory);
        JobsDirectory = Path.Combine(ConfigDirectory, "jobs");
        HistoryDirectory = Path.Combine(ConfigDirectory, "history");
        LogsDirectory = Path.Combine(ConfigDirectory, "logs");
    }

    /// <summary>
    /// Creates the config directory and its subdirectories if they do not exist yet.
    /// Safe to call repeatedly.
    /// </summary>
    public void EnsureDirectories()
    {
        Directory.CreateDirectory(ConfigDirectory);
        Directory.CreateDirectory(JobsDirectory);
        Directory.CreateDirectory(HistoryDirectory);
        Directory.CreateDirectory(LogsDirectory);
    }

    internal static string ResolveConfigDirectory(
        DesktopPlatform platform,
        Func<string, string?> getEnvironmentVariable,
        string userHomeDirectory)
    {
        return platform switch
        {
            DesktopPlatform.MacOS => Path.Combine(userHomeDirectory,
                "Library", "Application Support", "Taskington"),
            DesktopPlatform.Linux => Path.Combine(
                ResolveLinuxConfigBase(getEnvironmentVariable, userHomeDirectory),
                "taskington"),
            DesktopPlatform.Windows => Path.Combine(
                getEnvironmentVariable("APPDATA")
                    ?? throw new InvalidOperationException(
                        "The APPDATA environment variable is not set."),
                "Taskington"),
            _ => throw new ArgumentOutOfRangeException(nameof(platform)),
        };
    }

    private static string ResolveLinuxConfigBase(
        Func<string, string?> getEnvironmentVariable,
        string userHomeDirectory)
    {
        var xdgConfigHome = getEnvironmentVariable("XDG_CONFIG_HOME");
        return string.IsNullOrWhiteSpace(xdgConfigHome)
            ? Path.Combine(userHomeDirectory, ".config")
            : xdgConfigHome;
    }

    private static DesktopPlatform DetectPlatform()
    {
        if (OperatingSystem.IsMacOS())
        {
            return DesktopPlatform.MacOS;
        }

        if (OperatingSystem.IsWindows())
        {
            return DesktopPlatform.Windows;
        }

        return DesktopPlatform.Linux;
    }

    private static string GetUserHomeDirectory()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    }
}
