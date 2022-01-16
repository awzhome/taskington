using System;
using System.Collections.Generic;
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
                PlanEvents.PreCheckPlanExecution.Push(plan);
                PlanEvents.PlanHasErrorsUpdated.Push(plan, false, null);
                PlanEvents.PlanIsRunningUpdated.Push(plan, false);
                PlanEvents.PlanStatusTextUpdated.Push(plan, "Not run yet");
            }
        }

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
                        PlanEvents.PlanIsRunningUpdated.Push(plan, false);
                        PlanEvents.PlanStatusTextUpdated.Push(plan, "Finished successfully");
                    }
                });
            }
        }
    }
}
