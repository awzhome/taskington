using System.Linq;
using Taskington.Base.Events;
using Taskington.Base.TinyBus;

namespace Taskington.Base.Log
{
    class LogConfiguration
    {
        private readonly IEventBus eventBus;
        private readonly ILog log;

        public LogConfiguration(IEventBus eventBus, ILog log)
        {
            this.eventBus = eventBus;
            this.log = log;

            UpdateMinimumLevel();
            eventBus.Subscribe<ConfigurationReloaded>(e => UpdateMinimumLevel());
        }

        private void UpdateMinimumLevel()
        {
            var logLevel = eventBus.Request<GetConfigValue, string?>(new("log")).First();
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
