using System.Diagnostics;
using Taskington.Base.Config;
using Taskington.Base.Events;
using Taskington.Base.Extension;
using Taskington.Base.Log;
using Taskington.Base.Plans;
using Taskington.Base.Service;
using Taskington.Base.Steps;
using Taskington.Base.SystemOperations;
using Taskington.Base.TinyBus;

[assembly: TaskingtonExtension(typeof(Taskington.Base.BaseExtension))]

namespace Taskington.Base
{
    class BaseExtension : ITaskingtonExtension
    {
        ILog? log;

        public void Initialize(IEventBus eventBus, IHandlerStore handlerStore)
        {
            log = new FileLog();

            var configurationProvider = new YamlFileConfigurationProvider();
            var configurationReader = new YamlConfigurationReader(configurationProvider);
            var configurationWriter = new YamlConfigurationWriter(configurationProvider);
            var configurationManager = new ConfigurationManager(eventBus, configurationReader, configurationWriter);
            var logConfiguration = new LogConfiguration(eventBus, log);

            var systemOperations = new WindowsSystemOperations(eventBus);
            var syncStepExecution = new SyncStepExecution(eventBus);
            var planExecution = new PlanExecution(eventBus);

            handlerStore.Add(
                configurationManager,
                logConfiguration,
                configurationProvider,
                configurationReader,
                configurationWriter,
                systemOperations,
                syncStepExecution,
                planExecution);
        }
    }
}
