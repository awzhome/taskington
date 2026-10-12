using System;
using System.IO;
using System.Linq;
using Serilog;
using Serilog.Events;
using Taskington.Base.Platform;

namespace Taskington.Base.Logging;

/// <summary>
/// Bootstraps the global Serilog logger with file sinks under the platform logs/
/// directory (spec chapter 10): application-*.log receives all events, errors-*.log
/// receives errors only. Verbose level is enabled for development via Debug build
/// configuration or the --verbose startup flag; logging is not user-configurable
/// in the MVP.
/// </summary>
public static class LoggingBootstrap
{
    public const string VerboseFlag = "--verbose";

    private const string ApplicationLogFileName = "application-.log";
    private const string ErrorsLogFileName = "errors-.log";

    /// <summary>
    /// Configures the global logger with sinks under the platform logs/ directory
    /// and the verbose level derived from the startup arguments.
    /// </summary>
    public static void Configure(PlatformDirectoryService platformDirectories, params string[] startupArgs)
    {
        Configure(platformDirectories.LogsDirectory, IsVerboseEnabled(startupArgs));
    }

    /// <summary>
    /// Configures the global logger with file sinks in the given directory, creating
    /// the directory and both log files on start.
    /// </summary>
    internal static void Configure(string logsDirectory, bool verbose)
    {
        Directory.CreateDirectory(logsDirectory);
        CreateTodaysLogFiles(logsDirectory);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(verbose ? LogEventLevel.Verbose : LogEventLevel.Information)
            .WriteTo.File(
                Path.Combine(logsDirectory, ApplicationLogFileName),
                rollingInterval: RollingInterval.Day)
            .WriteTo.Logger(errors => errors
                .MinimumLevel.Error()
                .WriteTo.File(
                    Path.Combine(logsDirectory, ErrorsLogFileName),
                    rollingInterval: RollingInterval.Day))
            .CreateLogger();

        Log.Information("Logging initialized (verbose: {Verbose})", verbose);
    }

    /// <summary>
    /// Serilog creates rolling files lazily on the first matching event; create
    /// today's files up front so both exist on application start.
    /// </summary>
    private static void CreateTodaysLogFiles(string logsDirectory)
    {
        string today = DateTime.Now.ToString("yyyyMMdd");
        TouchFile(Path.Combine(logsDirectory, "application-" + today + ".log"));
        TouchFile(Path.Combine(logsDirectory, "errors-" + today + ".log"));
    }

    private static void TouchFile(string path)
    {
        if (!File.Exists(path))
        {
            using (File.Create(path))
            {
            }
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
    public static void ShutDown()
    {
        Log.CloseAndFlush();
    }
}
