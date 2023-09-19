using System.Linq;
using Taskington.Base.Config;

namespace Taskington.Base.Log;

public interface ILogConfiguration
{
    void UpdateMinimumLevel(string? defaultLevel = default);
}

class LogConfiguration : ILogConfiguration
{
    private readonly ILog log;
    private readonly IConfigurationManager configurationManager;

    public LogConfiguration(ILog log, IConfigurationManager configurationManager)
    {
        this.log = log;
        this.configurationManager = configurationManager;
        UpdateMinimumLevel("info");
        configurationManager.ConfigurationReloaded += (sender, e) => UpdateMinimumLevel();
    }

    public void UpdateMinimumLevel(string? defaultLevel = default)
    {
        var logLevel = defaultLevel ?? configurationManager.GetValue("log");
        (log as IReconfigurableLog)?.SetMiminumLevel(logLevel switch
        {
            "verbose" => LogLevel.Verbose,
            "warning" => LogLevel.Warning,
            "error" => LogLevel.Error,
            _ => LogLevel.Info,
        });
    }
}
