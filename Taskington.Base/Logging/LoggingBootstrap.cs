using System;
using System.IO;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Taskington.Base.FileSystem;
using Taskington.Base.Platform;

namespace Taskington.Base.Logging;

/// <summary>
/// Configures the global Serilog logger with file sinks under the platform logs/
/// directory (spec chapter 10): application-*.log receives all events, errors-*.log
/// receives errors only. Verbose level is enabled for development via Debug build
/// configuration or the --verbose startup flag; logging is not user-configurable
/// in the MVP.
/// </summary>
public sealed class LoggingBootstrap
{
    public const string VerboseFlag = "--verbose";

    private const string ApplicationLogFileName = "application-.log";
    private const string ErrorsLogFileName = "errors-.log";

    private readonly IFileSystemService fileSystem;

    public LoggingBootstrap(IFileSystemService fileSystem)
    {
        this.fileSystem = fileSystem;
    }

    /// <summary>
    /// Configures the global logger with sinks under the platform logs/ directory
    /// and the verbose level derived from the startup arguments.
    /// </summary>
    public void Configure(PlatformDirectoryService platformDirectories, params string[] startupArgs)
    {
        Configure(platformDirectories.LogsDirectory, IsVerboseEnabled(startupArgs));
    }

    /// <summary>
    /// Configures the global logger with file sinks in the given directory, creating
    /// the directory and both log files on start.
    /// </summary>
    internal void Configure(string logsDirectory, bool verbose)
    {
        fileSystem.CreateDirectory(logsDirectory);
        CreateTodaysLogFiles(logsDirectory);

        Log.Logger = BuildLoggerConfiguration(
            verbose,
            sinks => sinks.File(
                Path.Combine(logsDirectory, ApplicationLogFileName),
                rollingInterval: RollingInterval.Day),
            sinks => sinks.File(
                Path.Combine(logsDirectory, ErrorsLogFileName),
                rollingInterval: RollingInterval.Day))
            .CreateLogger();
    }

    /// <summary>
    /// The logging pipeline structure (spec chapter 10): minimum level from the
    /// verbose flag, all events to the application sink, error events to an
    /// additional errors sink. The sinks themselves are supplied by the caller,
    /// which allows tests to run without writing files.
    /// </summary>
    internal static LoggerConfiguration BuildLoggerConfiguration(
        bool verbose,
        Action<LoggerSinkConfiguration> writeApplicationSink,
        Action<LoggerSinkConfiguration> writeErrorSink)
    {
        var configuration = new LoggerConfiguration()
            .MinimumLevel.Is(verbose ? LogEventLevel.Verbose : LogEventLevel.Information);
        writeApplicationSink(configuration.WriteTo);
        configuration.WriteTo.Logger(errors =>
        {
            errors.MinimumLevel.Error();
            writeErrorSink(errors.WriteTo);
        });
        return configuration;
    }

    /// <summary>
    /// Serilog creates rolling files lazily on the first matching event; create
    /// today's files up front so both exist on application start.
    /// </summary>
    private void CreateTodaysLogFiles(string logsDirectory)
    {
        string today = DateTime.Now.ToString("yyyyMMdd");
        TouchFile(Path.Combine(logsDirectory, "application-" + today + ".log"));
        TouchFile(Path.Combine(logsDirectory, "errors-" + today + ".log"));
    }

    private void TouchFile(string path)
    {
        if (!fileSystem.FileExists(path))
        {
            fileSystem.CreateFile(path);
        }
    }

    /// <summary>
    /// True when verbose logging is enabled for this run: Debug builds always log
    /// verbosely; Release/Production builds require the --verbose startup flag.
    /// </summary>
    internal static bool IsVerboseEnabled(params string[] startupArgs)
    {
#if DEBUG
        return true;
#else
        return IsVerboseFlagRequested(startupArgs);
#endif
    }

    internal static bool IsVerboseFlagRequested(params string[] startupArgs)
    {
        return startupArgs.Contains(VerboseFlag, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Flushes and releases the global logger; call on application shutdown.
    /// </summary>
    public void ShutDown()
    {
        Log.CloseAndFlush();
    }
}
