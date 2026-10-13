using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Taskington.Base.FileSystem;
using Taskington.Base.Logging;
using Taskington.Base.Platform;
using Xunit;

namespace Taskington.Base.Tests;

public class LoggingBootstrapTests : IDisposable
{
    private readonly string logsDirectory = Path.Combine(
        Path.GetTempPath(), "taskington-tests-" + Guid.NewGuid());
    private readonly ILogger previousLogger = Log.Logger;
    private readonly LoggingBootstrap bootstrap = new(new FileSystemService());

    public void Dispose()
    {
        Log.CloseAndFlush();
        Log.Logger = previousLogger;

        if (Directory.Exists(logsDirectory))
        {
            Directory.Delete(logsDirectory, true);
        }
    }

    private static void ConfigureMemoryLogger(
        MemorySink applicationSink, MemorySink errorsSink, bool verbose)
    {
        Log.Logger = LoggingBootstrap.BuildLoggerConfiguration(
            verbose,
            sinks => sinks.Sink(applicationSink),
            sinks => sinks.Sink(errorsSink))
            .CreateLogger();
    }

    private static string TodaysApplicationLogPath(string logsDirectory)
    {
        return Path.Combine(logsDirectory, "application-" + DateTime.Now.ToString("yyyyMMdd") + ".log");
    }

    private static string TodaysErrorsLogPath(string logsDirectory)
    {
        return Path.Combine(logsDirectory, "errors-" + DateTime.Now.ToString("yyyyMMdd") + ".log");
    }

    [Fact]
    public void ConfigureCreatesApplicationAndErrorsLogFilesOnStart()
    {
        bootstrap.Configure(logsDirectory, verbose: false);

        Assert.Single(Directory.GetFiles(logsDirectory, "application-*.log"));
        Assert.Single(Directory.GetFiles(logsDirectory, "errors-*.log"));
    }

    [Fact]
    public void ConfigureFromPlatformDirectoriesWritesLogFilesOnStart()
    {
        string tempHome = Path.Combine(Path.GetTempPath(), "taskington-tests-" + Guid.NewGuid());
        try
        {
            var platformDirectories = new PlatformDirectoryService(
                DesktopPlatform.MacOS, _ => null, tempHome);
            platformDirectories.EnsureDirectories();

            bootstrap.Configure(platformDirectories);
            Log.Error("error event");

            Assert.Single(Directory.GetFiles(platformDirectories.LogsDirectory, "application-*.log"));
            string errorsLog = Directory.GetFiles(platformDirectories.LogsDirectory, "errors-*.log").Single();
            Assert.Contains("error event", File.ReadAllText(errorsLog));
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
    public void NonVerboseConfigurationFiltersOutDebugEvents()
    {
        var applicationSink = new MemorySink();
        var errorsSink = new MemorySink();
        ConfigureMemoryLogger(applicationSink, errorsSink, verbose: false);

        Log.Debug("debug event");

        Assert.Empty(applicationSink.Events);
        Assert.Empty(errorsSink.Events);
    }

    [Fact]
    public void VerboseConfigurationWritesDebugEvents()
    {
        var applicationSink = new MemorySink();
        var errorsSink = new MemorySink();
        ConfigureMemoryLogger(applicationSink, errorsSink, verbose: true);

        Log.Debug("debug event");

        Assert.Single(applicationSink.Events);
        Assert.Empty(errorsSink.Events);
    }

    [Fact]
    public void InformationEventsGoToApplicationLogOnly()
    {
        var applicationSink = new MemorySink();
        var errorsSink = new MemorySink();
        ConfigureMemoryLogger(applicationSink, errorsSink, verbose: false);

        Log.Information("information event");

        Assert.Single(applicationSink.Events);
        Assert.Empty(errorsSink.Events);
    }

    [Fact]
    public void ErrorEventsGoToErrorsLogAndApplicationLog()
    {
        var applicationSink = new MemorySink();
        var errorsSink = new MemorySink();
        ConfigureMemoryLogger(applicationSink, errorsSink, verbose: false);

        Log.Error("error event");

        Assert.Single(applicationSink.Events);
        Assert.Single(errorsSink.Events);
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

    [Fact]
    public void ConfigureUsesFileSystemToCreateLogsDirectoryAndBothLogFiles()
    {
        var fileSystem = new RecordingFileSystem();
        var platformDirectories = new PlatformDirectoryService(
            DesktopPlatform.MacOS, _ => null, "/test/home");
        var mockBootstrap = new LoggingBootstrap(fileSystem);

        mockBootstrap.Configure(platformDirectories);

        Assert.Contains(platformDirectories.LogsDirectory, fileSystem.CreatedDirectories);
        Assert.Contains(TodaysApplicationLogPath(platformDirectories.LogsDirectory), fileSystem.CreatedFiles);
        Assert.Contains(TodaysErrorsLogPath(platformDirectories.LogsDirectory), fileSystem.CreatedFiles);
    }

    [Fact]
    public void ConfigureLeavesExistingLogFilesUntouched()
    {
        var fileSystem = new RecordingFileSystem { AllFilesExist = true };
        var platformDirectories = new PlatformDirectoryService(
            DesktopPlatform.MacOS, _ => null, "/test/home");
        var mockBootstrap = new LoggingBootstrap(fileSystem);

        mockBootstrap.Configure(platformDirectories);

        Assert.Empty(fileSystem.CreatedFiles);
        Assert.Contains(TodaysApplicationLogPath(platformDirectories.LogsDirectory), fileSystem.CheckedFiles);
        Assert.Contains(TodaysErrorsLogPath(platformDirectories.LogsDirectory), fileSystem.CheckedFiles);
    }

    private sealed class RecordingFileSystem : IFileSystemService
    {
        public bool AllFilesExist { get; set; }

        public List<string> CreatedDirectories { get; } = new();
        public List<string> CheckedFiles { get; } = new();
        public List<string> CreatedFiles { get; } = new();

        public void CreateDirectory(string path)
        {
            CreatedDirectories.Add(path);
        }

        public bool FileExists(string path)
        {
            CheckedFiles.Add(path);
            return AllFilesExist;
        }

        public void CreateFile(string path)
        {
            CreatedFiles.Add(path);
        }

        public string ReadAllText(string path)
        {
            throw new NotSupportedException("Not used in this test.");
        }

        public void WriteAllText(string path, string contents)
        {
            throw new NotSupportedException("Not used in this test.");
        }

        public void DeleteFile(string path)
        {
            throw new NotSupportedException("Not used in this test.");
        }

        public void MoveFile(string sourcePath, string destinationPath, bool overwrite)
        {
            throw new NotSupportedException("Not used in this test.");
        }

        public string[] GetFiles(string directoryPath, string searchPattern)
        {
            throw new NotSupportedException("Not used in this test.");
        }
    }

    /// <summary>
    /// In-memory Serilog sink; collects emitted events instead of writing files.
    /// </summary>
    private sealed class MemorySink : ILogEventSink
    {
        public List<LogEvent> Events { get; } = new();

        public void Emit(LogEvent logEvent)
        {
            Events.Add(logEvent);
        }
    }
}
