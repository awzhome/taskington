using System;
using System.IO;
using Taskington.Base.Platform;
using Xunit;

namespace Taskington.Base.Tests;

public class PlatformDirectoryServiceTests
{
    [Fact]
    public void ResolvesMacOsConfigDirectoryInApplicationSupport()
    {
        var service = new PlatformDirectoryService(
            DesktopPlatform.MacOS, _ => null, "/Users/test");

        Assert.Equal(
            Path.Combine("/Users/test", "Library", "Application Support", "Taskington"),
            service.ConfigDirectory);
    }

    [Fact]
    public void ResolvesLinuxConfigDirectoryFromXdgConfigHome()
    {
        var service = new PlatformDirectoryService(
            DesktopPlatform.Linux, name => name == "XDG_CONFIG_HOME" ? "/custom/config" : null,
            "/home/test");

        Assert.Equal(Path.Combine("/custom/config", "taskington"), service.ConfigDirectory);
    }

    [Fact]
    public void ResolvesLinuxConfigDirectoryToDotConfigWithoutXdgConfigHome()
    {
        var service = new PlatformDirectoryService(
            DesktopPlatform.Linux, _ => null, "/home/test");

        Assert.Equal(
            Path.Combine("/home/test", ".config", "taskington"),
            service.ConfigDirectory);
    }

    [Fact]
    public void ResolvesWindowsConfigDirectoryFromAppData()
    {
        var service = new PlatformDirectoryService(
            DesktopPlatform.Windows, name => name == "APPDATA"
                ? Path.Combine("C:", "Users", "test", "AppData", "Roaming")
                : null,
            Path.Combine("C:", "Users", "test"));

        Assert.Equal(
            Path.Combine("C:", "Users", "test", "AppData", "Roaming", "Taskington"),
            service.ConfigDirectory);
    }

    [Fact]
    public void WindowsResolutionThrowsWithoutAppData()
    {
        Assert.Throws<InvalidOperationException>(() => new PlatformDirectoryService(
            DesktopPlatform.Windows, _ => null, Path.Combine("C:", "Users", "test")));
    }

    [Fact]
    public void JobsAndLogsDirectoriesAreSubdirectoriesOfConfigDirectory()
    {
        var service = new PlatformDirectoryService(
            DesktopPlatform.MacOS, _ => null, "/Users/test");

        Assert.Equal(Path.Combine(service.ConfigDirectory, "jobs"), service.JobsDirectory);
        Assert.Equal(Path.Combine(service.ConfigDirectory, "logs"), service.LogsDirectory);
    }

    [Fact]
    public void DefaultConstructorResolvesConfigDirectoryForCurrentPlatform()
    {
        var service = new PlatformDirectoryService();

        string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string expected;
        if (OperatingSystem.IsMacOS())
        {
            expected = Path.Combine(home, "Library", "Application Support", "Taskington");
        }
        else if (OperatingSystem.IsWindows())
        {
            expected = Path.Combine(
                Environment.GetEnvironmentVariable("APPDATA")!, "Taskington");
        }
        else
        {
            string xdgConfigHome = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME")!;
            expected = Path.Combine(
                string.IsNullOrWhiteSpace(xdgConfigHome) ? Path.Combine(home, ".config") : xdgConfigHome,
                "taskington");
        }

        Assert.Equal(expected, service.ConfigDirectory);
    }

    [Fact]
    public void EnsureDirectoriesCreatesConfigJobsAndLogsDirectories()
    {
        string tempHome = Path.Combine(Path.GetTempPath(), "taskington-tests-" + Guid.NewGuid());
        try
        {
            var service = new PlatformDirectoryService(
                DesktopPlatform.MacOS, _ => null, tempHome);

            service.EnsureDirectories();

            Assert.True(Directory.Exists(service.ConfigDirectory));
            Assert.True(Directory.Exists(service.JobsDirectory));
            Assert.True(Directory.Exists(service.LogsDirectory));
        }
        finally
        {
            if (Directory.Exists(tempHome))
            {
                Directory.Delete(tempHome, true);
            }
        }
    }

    [Fact]
    public void EnsureDirectoriesIsSafeToCallRepeatedly()
    {
        string tempHome = Path.Combine(Path.GetTempPath(), "taskington-tests-" + Guid.NewGuid());
        try
        {
            var service = new PlatformDirectoryService(
                DesktopPlatform.Linux, _ => null, tempHome);

            service.EnsureDirectories();
            service.EnsureDirectories();

            Assert.True(Directory.Exists(service.JobsDirectory));
            Assert.True(Directory.Exists(service.LogsDirectory));
        }
        finally
        {
            if (Directory.Exists(tempHome))
            {
                Directory.Delete(tempHome, true);
            }
        }
    }
}
