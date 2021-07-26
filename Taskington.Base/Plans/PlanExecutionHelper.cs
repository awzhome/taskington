using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taskington.Base.Events;
using Taskington.Base.Service;
using Taskington.Base.Steps;
using Taskington.Base.SystemOperations;

namespace Taskington.Base.Plans
{
    public class PlanExecutionHelper
    {
        private readonly IAppServiceProvider serviceProvider;
        private readonly ISystemOperations systemOperations;
        private readonly ApplicationEvents events;

        public PlanExecutionHelper(IAppServiceProvider serviceProvider, ISystemOperations systemOperations, ApplicationEvents events)
        {
            this.serviceProvider = serviceProvider;
            this.systemOperations = systemOperations;
            this.events = events;
        }

#pragma warning disable CA1822 // Mark members as static
        public bool CanExecute(Plan plan)
        {
#if SYS_OPS_DRYRUN
            return true;
#else
            var placeholders = new Placeholders();
            systemOperations.LoadSystemPlaceholders(placeholders);

            var stepTypes = new HashSet<string>(plan.Steps.Select(step => step.StepType));
            return
                !stepTypes.Select(type => serviceProvider.Get<IStepExecution>(s => s.Type == type)
                    ?.CanExecuteSupportedSteps(plan.Steps, placeholders))
                .Any(result => !(result ?? true));
#endif
        }
#pragma warning restore CA1822 // Mark members as static

        public async Task ExecuteAsync(Plan plan)
        {
            await Task.Run(() =>
            {
                try
                {
                    events.OnPlanIsRunning(plan, true);

                    var placeholders = new Placeholders();
                    systemOperations.LoadSystemPlaceholders(placeholders);

                    int stepsFinished = 0;
                    events.OnPlanProgress(plan, 0);
                    int planProgress = 0;
                    foreach (var step in plan.Steps)
                    {
                        var stepExecution = serviceProvider.Get<IStepExecution>(s => s.Type == step.StepType);
                        if (stepExecution != null)
                        {
                            stepExecution.Execute(step, placeholders,
                                progress => events.OnPlanProgress(plan, planProgress + progress / plan.Steps.Count()),
                                text => events.OnPlanStatusText(plan, text));
                            stepsFinished++;
                            planProgress = stepsFinished * 100 / plan.Steps.Count();
                            events.OnPlanProgress(plan, planProgress);
                        }
                        else
                        {
                            events.OnPlanHasErrors(plan, true, $"Unknown execution step '{step.StepType}'");
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    events.OnPlanHasErrors(plan, true, ex.Message);
                }
                finally
                {
                    events.OnPlanIsRunning(plan, false);
                    events.OnPlanStatusText(plan, "Finished successfully");
                }
            });
        }
    }
}
