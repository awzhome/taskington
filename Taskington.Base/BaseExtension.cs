using Taskington.Base.Config;
using Taskington.Base.Extension;
using Taskington.Base.Log;
using Taskington.Base.Plans;
using Taskington.Base.Steps;
using Taskington.Base.SystemOperations;

[assembly: TaskingtonExtension(typeof(Taskington.Base.BaseExtension))]

namespace Taskington.Base
{
    class BaseExtension : ITaskingtonExtension
    {
        ILog? log;

        public void Initialize(IHandlerStore handlerStore)
        {
            log = new FileLog();

            var configurationProvider = new YamlFileConfigurationProvider();
            var configurationReader = new YamlConfigurationReader(configurationProvider);
            var configurationWriter = new YamlConfigurationWriter(configurationProvider);
            var configurationManager = new ConfigurationManager(configurationReader, configurationWriter);
            var logConfiguration = new LogConfiguration(log);

            handlerStore.Add(
                configurationManager,
                logConfiguration,
                configurationProvider,
                configurationReader,
                configurationWriter,
                OsSpecificSystemOperations.Create(),
                new SyncStepExecution(),
                new PlanExecution());
        }
    }
}
