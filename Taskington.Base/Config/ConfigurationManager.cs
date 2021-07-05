﻿using PPBackup.Base.Plans;
using PPBackup.Base.Service;
using PPBackup.Base.Steps;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PPBackup.Base.Config
{
    public class ConfigurationManager : IAutoInitializable
    {
        private static readonly object configurationLock = new();

        private bool isInitialized = false;
        private bool reloadDelayed = false;
        private readonly HashSet<BackupPlan> runningPlans = new();

        private readonly IAppServiceProvider serviceProvider;
        private readonly ApplicationEvents applicationEvents;
        private readonly YamlConfigurationReader configurationReader;
        private readonly YamlConfigurationWriter configurationWriter;

        public ConfigurationManager(
            IAppServiceProvider serviceProvider,
            ApplicationEvents applicationEvents,
            YamlConfigurationReader configurationReader,
            YamlConfigurationWriter configurationWriter)
        {
            this.serviceProvider = serviceProvider;
            this.applicationEvents = applicationEvents;
            this.configurationReader = configurationReader;
            this.configurationWriter = configurationWriter;

            applicationEvents.ConfigurationChanged += OnConfigurationChanged;
        }

        private readonly List<ExecutableBackupPlan> executablePlans = new();
        public IEnumerable<ExecutableBackupPlan> ExecutablePlans => executablePlans;

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
            bool configReloaded = false;
            lock (configurationLock)
            {
                configReloaded = TryReloadConfiguration();
            }

            if (configReloaded)
            {
                applicationEvents.OnConfigurationReloaded();
            }
        }

        private bool TryReloadConfiguration()
        {
            if (runningPlans.Count == 0)
            {
                foreach (var executablePlan in executablePlans)
                {
                    (executablePlan.Execution as IDisposable)?.Dispose();
                }
                executablePlans.Clear();

                ReadConfiguration();
                return true;
            }
            else
            {
                if (!reloadDelayed)
                {
                    applicationEvents.OnConfigurationReloadDelayed(true);
                }
                reloadDelayed = true;
                return false;
            }
        }

        private void ReadConfiguration()
        {
            executablePlans.AddRange(configurationReader.Read().Select(CreateExecutablePlan));
        }

        private ExecutableBackupPlan CreateExecutablePlan(BackupPlan plan)
        {
            if (plan.Steps.OfType<InvalidBackupStep>().Any())
            {
                var events = CreatePlanExecutionEvents(plan);
                return new ExecutableBackupPlan(
                    plan,
                    new InvalidPlanExecution(events, "Plan contains invalid steps."),
                    events);
            }
            else
            {
                var planExecutionCreator = serviceProvider.Get<IPlanExecutionCreator>(
                    execution => execution.RunType == plan.RunType);
                if (planExecutionCreator == null)
                {
                    var events = CreatePlanExecutionEvents(plan);
                    return new ExecutableBackupPlan(
                        plan,
                        new InvalidPlanExecution(events, $"Unknown backup plan run type '{plan.RunType}'"),
                        events);
                }
                else
                {
                    var events = CreatePlanExecutionEvents(plan);
                    return new ExecutableBackupPlan(
                         plan,
                         planExecutionCreator.Create(plan, events),
                         events);
                }
            }
        }

        private PlanExecutionEvents CreatePlanExecutionEvents(BackupPlan plan)
        {
            var events = new PlanExecutionEvents(plan);
            events.IsRunning += OnPlanIsRunningUpdated;
            return events;
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
                bool configReloaded = false;
                lock (configurationLock)
                {
                    runningPlans.Remove(e.BackupPlan);
                    if ((runningPlans.Count == 0) && reloadDelayed)
                    {
                        applicationEvents.OnConfigurationReloadDelayed(false);
                        reloadDelayed = false;
                        configReloaded = TryReloadConfiguration();
                    }
                }
                if (configReloaded)
                {
                    applicationEvents.OnConfigurationReloaded();
                }
            }
        }

        public void SaveConfiguration()
        {
            configurationWriter.Write(executablePlans.Select(ep => ep.BackupPlan));
        }

        public ExecutableBackupPlan InsertPlan(int index, BackupPlan newPlan)
        {
            var newExecutablePlan = CreateExecutablePlan(newPlan);
            if (index > -1)
            {
                executablePlans.Insert(index, newExecutablePlan);
            }
            else
            {
                executablePlans.Add(newExecutablePlan);
            }

            return newExecutablePlan;
        }

        public void RemovePlan(ExecutableBackupPlan executablePlan)
        {
            int index = executablePlans.IndexOf(executablePlan);
            if (index > -1)
            {
                (executablePlan.Execution as IDisposable)?.Dispose();
                executablePlans.RemoveAt(index);
            }
        }

        public ExecutableBackupPlan ReplacePlan(ExecutableBackupPlan executablePlan, BackupPlan newPlan)
        {
            var newExecutablePlan = CreateExecutablePlan(newPlan);
            int index = executablePlans.IndexOf(executablePlan);
            if (index > -1)
            {
                (executablePlan.Execution as IDisposable)?.Dispose();
                executablePlans.RemoveAt(index);
                executablePlans.Insert(index, newExecutablePlan);
            }
            else
            {
                executablePlans.Add(newExecutablePlan);
            }

            return newExecutablePlan;
        }
    }
}