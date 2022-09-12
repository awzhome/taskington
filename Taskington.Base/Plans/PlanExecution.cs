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
            PlanMessages.NotifyInitialPlanStates.Subscribe(NotifyInitialStates);
            PlanMessages.ExecutePlan.Subscribe(Execute);
        }

        private void NotifyInitialStates(Plan plan)
        {
            if (plan.RunType == Plan.OnSelectionRunType)
            {
                PlanMessages.PreCheckPlanExecution.Push(plan);
                PlanMessages.PlanHasErrorsUpdated.Push(plan, false, null);
                PlanMessages.PlanIsRunningUpdated.Push(plan, false);
                PlanMessages.PlanStatusTextUpdated.Push(plan, "Not run yet");
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
                        PlanMessages.PlanIsRunningUpdated.Push(plan, true);

                        var placeholders = new LoadSystemPlaceholdersMessage().Request().First();

                        int stepsFinished = 0;
                        PlanMessages.PlanProgressUpdated.Push(plan, 0);
                        int planProgress = 0;
                        foreach (var step in plan.Steps)
                        {
                            PlanMessages.ExecuteStep.Push(step, placeholders,
                                progress => PlanMessages.PlanProgressUpdated.Push(plan, planProgress + progress / plan.Steps.Count()),
                                text => PlanMessages.PlanStatusTextUpdated.Push(plan, text));
                            stepsFinished++;
                            planProgress = stepsFinished * 100 / plan.Steps.Count();
                            PlanMessages.PlanProgressUpdated.Push(plan, planProgress);
                        }
                    }
                    catch (Exception ex)
                    {
                        PlanMessages.PlanHasErrorsUpdated.Push(plan, true, ex.Message);
                    }
                    finally
                    {
                        PlanMessages.PlanIsRunningUpdated.Push(plan, false);
                        PlanMessages.PlanStatusTextUpdated.Push(plan, "Finished successfully");
                    }
                });
            }
        }
    }
}
