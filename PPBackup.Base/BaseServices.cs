using PPBackup.Base.Config;
using PPBackup.Base.Plans;
using PPBackup.Base.Service;
using PPBackup.Base.Steps;
using PPBackup.Base.SystemOperations;
using System.Collections.Generic;

namespace PPBackup.Base
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
                .Bind<IPlanExecutionCreator, ManualPlanExecution.Creator>();
        }
    }
}
