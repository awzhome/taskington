using Taskington.Base.Config;
using Taskington.Base.Events;

namespace Taskington.Base.Log
{
    class LogConfiguration
    {
        public LogConfiguration(IApplicationEvents applicationEvents, ILog log, ConfigurationManager configurationManager)
        {
            applicationEvents.ConfigurationReloaded += (o, e) =>
            {
                (log as IReconfigurableLog)?.SetMiminumLevel(configurationManager.GetValue("log") switch
                {
                    "verbose" => LogLevel.Verbose,
                    "warning" => LogLevel.Warning,
                    "error" => LogLevel.Error,
                    _ => LogLevel.Info,
                });
            };
        }
    }
}
