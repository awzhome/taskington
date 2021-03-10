using PPBackup.Base.Plans;
using PPBackup.Base.Service;
using PPBackup.Base.Steps;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PPBackup.Base.Config
{
    class ConfigurationManager : IAutoInitializable
    {
        private static readonly object configurationLock = new();

        private bool isInitialized = false;
        private bool reloadDelayed = false;
        private readonly HashSet<BackupPlan> runningPlans = new();

        private readonly ApplicationServices services;
        private readonly ApplicationEvents applicationEvents;
        private readonly List<ExecutableBackupPlan> executablePlans;
        private readonly ScriptConfigurationReader scriptConfigurationReader;

        public ConfigurationManager(
            ApplicationServices services,
            ApplicationEvents applicationEvents,
            List<ExecutableBackupPlan> executablePlans,
            ScriptConfigurationReader scriptConfigurationReader)
        {
            this.services = services;
            this.applicationEvents = applicationEvents;
            this.executablePlans = executablePlans;
            this.scriptConfigurationReader = scriptConfigurationReader;

            applicationEvents.ConfigurationChanged += OnConfigurationChanged;
        }

        public void Initialize()
        {
            if (!isInitialized)
            {
                lock (configurationLock)
                {
                    ReadConfiguration();
                }
                isInitialized = true;
            }
        }

        private void OnConfigurationChanged(object? sender, EventArgs e)
        {
            lock (configurationLock)
            {
                TryReloadConfiguration();
            }
        }

        private void TryReloadConfiguration()
        {
            if (runningPlans.Count == 0)
            {
                foreach (var executablePlan in executablePlans)
                {
                    (executablePlan.Execution as IDisposable)?.Dispose();
                }
                executablePlans.Clear();

                ReadConfiguration();
                applicationEvents.ConfigurationReload();
            }
            else
            {
                if (!reloadDelayed)
                {
                    applicationEvents.ConfigurationReloadDelay(true);
                }
                reloadDelayed = true;
            }
        }

        private void ReadConfiguration()
        {
            var backupPlans = scriptConfigurationReader.Read();

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
                    var planExecutionCreator = services.Get<IPlanExecutionCreator>(
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

                events.IsRunningUpdated += OnPlanIsRunningUpdated;
            }
        }

        private void OnPlanIsRunningUpdated(object? sender, PlanIsRunningUpdatedEventArgs e)
        {
            if (e.IsRunning)
            {
                lock (configurationLock)
                {
                    runningPlans.Add(e.BackupPlan);
                }
            }
            else
            {
                lock (configurationLock)
                {
                    runningPlans.Remove(e.BackupPlan);
                    if ((runningPlans.Count == 0) && reloadDelayed)
                    {
                        applicationEvents.ConfigurationReloadDelay(false);
                        reloadDelayed = false;
                        TryReloadConfiguration();
                    }
                }
            }
        }
    }
}
