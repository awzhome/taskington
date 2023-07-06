using System.Linq;
using Taskington.Base.Config;

namespace Taskington.Base.Log
{
    class LogConfiguration
    {
        private readonly ILog log;

        public LogConfiguration(ILog log)
        {
            this.log = log;

            UpdateMinimumLevel("info");
            ConfigurationReloadedMessage.Subscribe(_ => UpdateMinimumLevel());
        }

        private void UpdateMinimumLevel(string? defaultLevel = default)
        {
            var logLevel = defaultLevel ?? new GetConfigValueMessage("log").Request().First();
            (log as IReconfigurableLog)?.SetMiminumLevel(logLevel switch
            {
                "verbose" => LogLevel.Verbose,
                "warning" => LogLevel.Warning,
                "error" => LogLevel.Error,
                _ => LogLevel.Info,
            });
        }
    }
}
