using PPBackup.Base.Config;
using PPBackup.Base.Plans;
using PPBackup.Base.Steps;
using PPBackup.Base.SystemOperations;
using System.Collections.Generic;

namespace PPBackup.Base
{
    static class BaseServices
    {
        public static void Bind(ApplicationServices services)
        {
            var applicationEvents = new ApplicationEvents();
            var executablePlans = new List<ExecutableBackupPlan>();

            services
                .With(applicationEvents)
                .With<IApplicationEvents>(applicationEvents)
                .With<IStreamReaderProvider, ScriptFileConfigurationProvider>()
                .With<ScriptConfigurationReader>()
                .With(SystemOperationsFactory.CreateSystemOperations)
                .With<IStepExecution, SyncStepExecution>()
                .With<PlanExecutionHelper>()
                .With<IPlanExecutionCreator, ManualPlanExecution.Creator>()
                .With(executablePlans)
                .With<IEnumerable<ExecutableBackupPlan>>(executablePlans);
        }
    }
}
