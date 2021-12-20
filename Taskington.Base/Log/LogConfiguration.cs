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

            UpdateMinimumLevel();
            ConfigurationEvents.ConfigurationReloaded.Subscribe(UpdateMinimumLevel);
        }

        private void UpdateMinimumLevel()
        {
            var logLevel = ConfigurationEvents.GetConfigValue.Request("log").First();
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
