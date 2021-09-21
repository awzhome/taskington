using Taskington.Base.Config;
using Taskington.Base.Events;
using Taskington.Base.Log;
using Taskington.Base.Plans;
using Taskington.Base.Service;
using Taskington.Base.Steps;
using Taskington.Base.SystemOperations;

[assembly: TaskingtonExtension(typeof(Taskington.Base.BaseServices))]

namespace Taskington.Base
{
    static class BaseServices
    {
        public static void Bind(IAppServiceBinder binder)
        {
            ApplicationEvents applicationEvents = new();

            binder
                .Bind(applicationEvents)
                .Bind<IApplicationEvents>(applicationEvents)
                .Bind<IStreamReaderProvider, YamlFileConfigurationProvider>()
                .Bind<IStreamWriterProvider, YamlFileConfigurationProvider>()
                .Bind<ConfigurationManager>()
                .Bind<ScriptConfigurationReader>()
                .Bind<YamlConfigurationReader>()
                .Bind<YamlConfigurationWriter>()
                .Bind(SystemOperationsFactory.CreateSystemOperations)
                .Bind<IStepExecution, SyncStepExecution>()
                .Bind<PlanExecutionHelper>()
                .Bind<IPlanExecutionCreator, ManualPlanExecution.Creator>()
                .Bind<ILog, FileLog>()
                .Bind<LogConfiguration>();
        }
    }
}
