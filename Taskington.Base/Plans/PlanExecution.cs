using System;
using System.Linq;
using System.Threading.Tasks;
using Taskington.Base.Events;
using Taskington.Base.SystemOperations;
using Taskington.Base.TinyBus;

namespace Taskington.Base.Plans
{
    internal class PlanExecution
    {
        private readonly IEventBus eventBus;

        internal PlanExecution(IEventBus eventBus)
        {
            this.eventBus = eventBus;

            eventBus
                .Subscribe<NotifyInitialPlanStates>(NotifyInitialStates)
                .Subscribe<ExecutePlan>(Execute);
        }

        private void NotifyInitialStates(NotifyInitialPlanStates e)
        {
            if (e.Plan.RunType == Plan.OnSelectionRunType)
            {
                eventBus
                    .Push(new PlanCanExecuteUpdated(e.Plan, CanExecute(e.Plan)))
                    .Push(new PlanHasErrorsUpdated(e.Plan, false))
                    .Push(new PlanIsRunningUpdated(e.Plan, false))
                    .Push(new PlanStatusTextUpdated(e.Plan, "Not run yet"));
            }
        }

#pragma warning disable CA1822 // Mark members as static
        public bool CanExecute(Plan plan)
        {
            // TODO
            //#if SYS_OPS_DRYRUN
            return true;
            //#else
            //            var placeholders = new Placeholders();
            //            systemOperations.LoadSystemPlaceholders(placeholders);

            //            var stepTypes = new HashSet<string>(plan.Steps.Select(step => step.StepType));
            //            return
            //                !stepTypes.Select(type => serviceProvider.Get<IStepExecution>(s => s.Type == type)
            //                    ?.CanExecuteSupportedSteps(plan.Steps, placeholders))
            //                .Any(result => !(result ?? true));
            //#endif
        }
#pragma warning restore CA1822 // Mark members as static

        private void Execute(ExecutePlan e)
        {
            if (e.Plan.IsValid)
            {
                Task.Run(() =>
                {
                    var plan = e.Plan;

                    try
                    {
                        eventBus.Push(new PlanIsRunningUpdated(plan, true));

                        var placeholders = eventBus.Request<LoadSystemPlaceholders, Placeholders>(new()).First();

                        int stepsFinished = 0;
                        eventBus.Push(new PlanProgressUpdated(plan, 0));
                        int planProgress = 0;
                        foreach (var step in plan.Steps)
                        {
                            eventBus.Push(new ExecuteStep(step, placeholders,
                                progress => eventBus.Push(new PlanProgressUpdated(plan, planProgress + progress / plan.Steps.Count())),
                                text => eventBus.Push(new PlanStatusTextUpdated(plan, text))));
                            stepsFinished++;
                            planProgress = stepsFinished * 100 / plan.Steps.Count();
                            eventBus.Push(new PlanProgressUpdated(plan, planProgress));
                        }
                    }
                    catch (Exception ex)
                    {
                        eventBus.Push(new PlanHasErrorsUpdated(plan, true, ex.Message));
                    }
                    finally
                    {
                        eventBus
                            .Push(new PlanIsRunningUpdated(plan, true))
                            .Push(new PlanStatusTextUpdated(plan, "Finished successfully"));
                    }
                });
            }
        }
    }
}
