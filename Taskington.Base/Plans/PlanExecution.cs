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
            NotifyInitialPlanStatesMessage.Subscribe(m => NotifyInitialStates(m.Plan));
            ExecutePlanMessage.Subscribe(m => Execute(m.Plan));
        }

        private void NotifyInitialStates(Plan plan)
        {
            if (plan.RunType == Plan.OnSelectionRunType)
            {
                new PreCheckPlanExecutionMessage(plan).Publish();
                new PlanStatusTextUpdateMessage(plan, "Not run yet").Publish();
                new PlanRunningUpdateMessage(plan, false).Publish();
                new PlanErrorUpdateMessage(plan, false, null).Publish();
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
                        new PlanRunningUpdateMessage(plan, true).Publish();

                        var placeholders = new LoadSystemPlaceholdersMessage().Request().First();

                        int stepsFinished = 0;
                        new PlanProgressUpdateMessage(plan, 0).Publish();
                        int planProgress = 0;
                        foreach (var step in plan.Steps)
                        {
                            new ExecuteStepMessage(step, placeholders,
                                progress => new PlanProgressUpdateMessage(plan, planProgress + progress / plan.Steps.Count()).Publish(),
                                text => new PlanStatusTextUpdateMessage(plan, text)).Publish();
                            stepsFinished++;
                            planProgress = stepsFinished * 100 / plan.Steps.Count();
                            new PlanProgressUpdateMessage(plan, planProgress).Publish();
                        }
                    }
                    catch (Exception ex)
                    {
                        new PlanErrorUpdateMessage(plan, true, ex.Message).Publish();
                    }
                    finally
                    {
                        new PlanRunningUpdateMessage(plan, false).Publish();
                        new PlanStatusTextUpdateMessage(plan, "Finished successfully").Publish();
                    }
                });
            }
        }
    }
}
