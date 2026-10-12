using System;
using System.IO;
using System.Linq;
using Serilog;
using Taskington.Base.Logging;
using Taskington.Base.Platform;
using Xunit;

namespace Taskington.Base.Tests;

public class LoggingBootstrapTests : IDisposable
{
    private readonly string logsDirectory = Path.Combine(
        Path.GetTempPath(), "taskington-tests-" + Guid.NewGuid());
    private readonly ILogger previousLogger = Log.Logger;

    public void Dispose()
    {
        Log.CloseAndFlush();
        Log.Logger = previousLogger;

        if (Directory.Exists(logsDirectory))
        {
            Directory.Delete(logsDirectory, true);
        }
    }

    private string ReadSingleLog(string searchPattern)
    {
        string logFile = Directory.GetFiles(logsDirectory, searchPattern).Single();
        return File.ReadAllText(logFile);
    }

    [Fact]
    public void ConfigureCreatesApplicationAndErrorsLogFilesOnStart()
    {
        LoggingBootstrap.Configure(logsDirectory, verbose: false);

        Assert.Single(Directory.GetFiles(logsDirectory, "application-*.log"));
        Assert.Single(Directory.GetFiles(logsDirectory, "errors-*.log"));
    }

    [Fact]
    public void ConfigureCreatesMissingLogsDirectory()
    {
        LoggingBootstrap.Configure(logsDirectory, verbose: false);

        Assert.True(Directory.Exists(logsDirectory));
    }

    [Fact]
    public void ConfigureWritesEventsToApplicationLog()
    {
        LoggingBootstrap.Configure(logsDirectory, verbose: false);
        Log.Information("application event");

        Assert.Contains("application event", ReadSingleLog("application-*.log"));
    }

    [Fact]
    public void ConfigureWritesErrorsToErrorsLogAndApplicationLog()
    {
        LoggingBootstrap.Configure(logsDirectory, verbose: false);
        Log.Error("error event");

        Assert.Contains("error event", ReadSingleLog("errors-*.log"));
        Assert.Contains("error event", ReadSingleLog("application-*.log"));
    }

    [Fact]
    public void NonVerboseConfigurationFiltersOutDebugEvents()
    {
        LoggingBootstrap.Configure(logsDirectory, verbose: false);
        Log.Debug("debug event");

        Assert.DoesNotContain("debug event", ReadSingleLog("application-*.log"));
    }

    [Fact]
    public void VerboseConfigurationWritesDebugEvents()
    {
        LoggingBootstrap.Configure(logsDirectory, verbose: true);
        Log.Debug("debug event");

        Assert.Contains("debug event", ReadSingleLog("application-*.log"));
    }

    [Fact]
    public void ConfigureFromPlatformDirectoriesUsesPlatformLogsDirectory()
    {
        string tempHome = Path.Combine(Path.GetTempPath(), "taskington-tests-" + Guid.NewGuid());
        try
        {
            var platformDirectories = new PlatformDirectoryService(
                DesktopPlatform.MacOS, _ => null, tempHome);
            platformDirectories.EnsureDirectories();

            LoggingBootstrap.Configure(platformDirectories);

            Assert.Single(Directory.GetFiles(platformDirectories.LogsDirectory, "application-*.log"));
            Assert.Single(Directory.GetFiles(platformDirectories.LogsDirectory, "errors-*.log"));
        }
        finally
        {
            Log.CloseAndFlush();
            Log.Logger = previousLogger;
            if (Directory.Exists(tempHome))
            {
                Directory.Delete(tempHome, true);
            }
        }
    }

    [Fact]
    public void IsVerboseFlagRequestedMatchesFlagCaseInsensitively()
    {
        Assert.True(LoggingBootstrap.IsVerboseFlagRequested("--verbose"));
        Assert.True(LoggingBootstrap.IsVerboseFlagRequested("--VERBOSE"));
    }

    [Fact]
    public void IsVerboseFlagRequestedReturnsFalseWithoutFlag()
    {
        Assert.False(LoggingBootstrap.IsVerboseFlagRequested(Array.Empty<string>()));
    }

    [Fact]
    public void IsVerboseEnabledIsTrueWithVerboseFlag()
    {
        Assert.True(LoggingBootstrap.IsVerboseEnabled("--verbose"));
    }
}
