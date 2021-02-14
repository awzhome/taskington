using PPBackup.Base.Config;
using PPBackup.Base.Plans;
using PPBackup.Base.Steps;
using PPBackup.Base.SystemOperations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PPBackup.Base
{
    public class Application
    {
        public ApplicationServices Services { get; }

        public Application()
        {
            Services = new ApplicationServices();

            RegisterDefaultServices();
        }

        private void RegisterDefaultServices()
        {
            var applicationEvents = new ApplicationEvents();
            var executablePlans = new List<ExecutableBackupPlan>();

            Services
                .With(this)
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

        public void Start()
        {
            Services.Start();
            Services.Get<ApplicationEvents>().ConfigurationChanged += OnConfigurationChanged;

            ReadConfiguration();
        }

        private void OnConfigurationChanged(object? sender, EventArgs e)
        {
            var applicationEvents = Services.Get<ApplicationEvents>();
            var executablePlans = Services.Get<List<ExecutableBackupPlan>>();
            foreach (var executablePlan in executablePlans)
            {
                if (executablePlan.Execution is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            executablePlans.Clear();

            ReadConfiguration();
            applicationEvents.ConfigurationReload();
        }

        private void ReadConfiguration()
        {
            var executablePlans = Services.Get<List<ExecutableBackupPlan>>();
            var backupPlans = Services.Get<ScriptConfigurationReader>().Read();

            foreach (var plan in backupPlans)
            {
                var events = new PlanExecutionEvents(plan);

                if (plan.Steps.OfType<InvalidBackupStep>().Any())
                {
                    executablePlans.Add(new ExecutableBackupPlan(
                        plan,
                        new InvalidPlanExecution(events, "Plan contains invalid steps."),
                        events));
                }
                else
                {
                    var planExecutionCreator = Services.Get<IPlanExecutionCreator>(
                        execution => execution.RunType == plan.RunType);
                    if (planExecutionCreator == null)
                    {
                        executablePlans.Add(new ExecutableBackupPlan(
                            plan,
                            new InvalidPlanExecution(events, $"Unknown backup plan run type '{plan.RunType}'"),
                            events));
                    }
                    else
                    {
                        executablePlans.Add(new ExecutableBackupPlan(
                            plan,
                            planExecutionCreator.Create(plan, events),
                            events));
                    }
                }
            }
        }

        public void NotifyInitialStates()
        {
            foreach (var execution in Services.Get<List<ExecutableBackupPlan>>().Select(executablePlan => executablePlan.Execution))
            {
                execution.NotifyInitialStates();
            }
        }
    }
}
