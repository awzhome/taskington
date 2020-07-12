﻿using PPBackup.Base.Config;
using PPBackup.Base.Executors;
using PPBackup.Base.SystemOperations;
using System.Collections.Generic;
using System.Numerics;

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
            var planExecutions = new List<IPlanExecution>();

            Services
                .With(this)
                .With<IConfigurationProvider, YamlFileConfigurationProvider>()
                .With<ConfigurationReader>()
                .With(SystemOperationsFactory.CreateSystemOperations)
                .With<IStepExecution, SyncStepExecution>()
                .With<PlanExecutionHelper>()
                .With<IPlanExecutionCreator, ManualPlanExecution.Creator>()
                .With(planExecutions)
                .With<IEnumerable<IPlanExecution>>(planExecutions);
        }

        public void Start()
        {
            Services.Start();

            var planExecutions = Services.Get<List<IPlanExecution>>();
            var backupPlans = Services.Get<ConfigurationReader>().Read();

            foreach (var plan in backupPlans)
            {
                var planExecutionCreator = Services.Get<IPlanExecutionCreator>(execution => execution.RunType == plan.RunType);
                if (planExecutionCreator == null)
                {
                    plan.HasErrors = true;
                    plan.ErrorMessage = $"Unknown backup plan run type '{plan.RunType}";
                }
                else
                {
                    planExecutions.Add(planExecutionCreator.Create(plan, new PlanExecutionStatus()));
                }
            }
        }
    }
}
