using Taskington.Base.Config;
using Taskington.Base.Events;
using Taskington.Base.Service;

namespace Taskington.Base.Log
{
    class LogConfiguration : IAutoInitializable
    {
        private readonly IApplicationEvents applicationEvents;
        private readonly ILog log;
        private readonly ConfigurationManager configurationManager;

        public LogConfiguration(IApplicationEvents applicationEvents, ILog log, ConfigurationManager configurationManager)
        {
            this.applicationEvents = applicationEvents;
            this.log = log;
            this.configurationManager = configurationManager;
        }

        public void Initialize()
        {
            UpdateMinimumLevel();
            applicationEvents.ConfigurationReloaded += (o, e) =>
            {
                UpdateMinimumLevel();
            };
        }

        private void UpdateMinimumLevel()
        {
            (log as IReconfigurableLog)?.SetMiminumLevel(configurationManager.GetValue("log") switch
            {
                "verbose" => LogLevel.Verbose,
                "warning" => LogLevel.Warning,
                "error" => LogLevel.Error,
                _ => LogLevel.Info,
            });
        }
    }
}
