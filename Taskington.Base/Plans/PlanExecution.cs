using System;
using System.Linq;
using System.Threading.Tasks;
using Taskington.Base.SystemOperations;

namespace Taskington.Base.Plans
{
    internal class PlanExecution
    {
        internal PlanExecution()
        {
            PlanEvents.NotifyInitialPlanStates.Subscribe(NotifyInitialStates);
            PlanEvents.ExecutePlan.Subscribe(Execute);
        }

        private void NotifyInitialStates(Plan plan)
        {
            if (plan.RunType == Plan.OnSelectionRunType)
            {
                PlanEvents.PlanCanExecuteUpdated.Push(plan, CanExecute(plan));
                PlanEvents.PlanHasErrorsUpdated.Push(plan, false, null);
                PlanEvents.PlanIsRunningUpdated.Push(plan, false);
                PlanEvents.PlanStatusTextUpdated.Push(plan, "Not run yet");
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

        private void Execute(Plan plan)
        {
            if (plan.IsValid)
            {
                Task.Run(() =>
                {
                    try
                    {
                        PlanEvents.PlanIsRunningUpdated.Push(plan, true);

                        var placeholders = SystemOperationsEvents.LoadSystemPlaceholders.Request().First();

                        int stepsFinished = 0;
                        PlanEvents.PlanProgressUpdated.Push(plan, 0);
                        int planProgress = 0;
                        foreach (var step in plan.Steps)
                        {
                            PlanEvents.ExecuteStep.Push(step, placeholders,
                                progress => PlanEvents.PlanProgressUpdated.Push(plan, planProgress + progress / plan.Steps.Count()),
                                text => PlanEvents.PlanStatusTextUpdated.Push(plan, text));
                            stepsFinished++;
                            planProgress = stepsFinished * 100 / plan.Steps.Count();
                            PlanEvents.PlanProgressUpdated.Push(plan, planProgress);
                        }
                    }
                    catch (Exception ex)
                    {
                        PlanEvents.PlanHasErrorsUpdated.Push(plan, true, ex.Message);
                    }
                    finally
                    {
                        PlanEvents.PlanIsRunningUpdated.Push(plan, true);
                        PlanEvents.PlanStatusTextUpdated.Push(plan, "Finished successfully");
                    }
                });
            }
        }
    }
}
