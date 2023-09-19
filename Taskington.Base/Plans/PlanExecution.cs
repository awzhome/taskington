using System;
using System.Linq;
using System.Threading.Tasks;
using Taskington.Base.Extension;
using Taskington.Base.Steps;
using Taskington.Base.SystemOperations;

namespace Taskington.Base.Plans;

internal class PlanExecution : IPlanExecution
{
    private readonly ISystemOperations systemOperations;
    private readonly IKeyedRegistry<IStepExecution> stepExecutions;

    public event EventHandler<PlanProgressUpdatedEventArgs>? PlanProgressUpdated;
    public event EventHandler<PlanStatusTextUpdatedEventArgs>? PlanStatusTextUpdated;
    public event EventHandler<PlanErrorUpdatedEventArgs>? PlanErrorUpdated;
    public event EventHandler<PlanCanExecuteUpdatedEventArgs>? PlanCanExecuteUpdated;
    public event EventHandler<PlanRunningUpdatedEventArgs>? PlanRunningUpdated;
    public event EventHandler<PlanPreCheckRequestedEventArgs>? PlanPreCheckRequested;

    internal PlanExecution(ISystemOperations systemOperations, IKeyedRegistry<IStepExecution> stepExecutions)
    {
        this.systemOperations = systemOperations;
        this.stepExecutions = stepExecutions;
    }

    public void NotifyInitialStates(Plan plan)
    {
        if (plan.RunType == Plan.OnSelectionRunType)
        {
            var planPreCheckRequestedEventArgs = new PlanPreCheckRequestedEventArgs(plan);
            PlanPreCheckRequested?.Invoke(this, planPreCheckRequestedEventArgs);
            PlanCanExecuteUpdated?.Invoke(this, new(plan, planPreCheckRequestedEventArgs.CanExecute));
            PlanStatusTextUpdated?.Invoke(this, new(plan, "Not run yet"));
            PlanRunningUpdated?.Invoke(this, new(plan, false));
            PlanErrorUpdated?.Invoke(this, new(plan, false, null));
        }
    }

    public void Execute(Plan plan)
    {
        if (plan.IsValid)
        {
            Task.Run(() =>
            {
                try
                {
                    PlanRunningUpdated?.Invoke(this, new(plan, true));

                    var placeholders = systemOperations.LoadSystemPlaceholders();

                    int stepsFinished = 0;
                    PlanProgressUpdated?.Invoke(this, new(plan, 0));
                    int planProgress = 0;
                    foreach (var step in plan.Steps)
                    {
                        stepExecutions.Get(step.StepType)?.Execute(step, placeholders,
                            progress => PlanProgressUpdated?.Invoke(this, new(plan, planProgress + progress / plan.Steps.Count())),
                            text => PlanStatusTextUpdated?.Invoke(this, new(plan, text)));
                        stepsFinished++;
                        planProgress = stepsFinished * 100 / plan.Steps.Count();
                        PlanProgressUpdated?.Invoke(this, new(plan, planProgress));
                    }
                }
                catch (Exception ex)
                {
                    PlanErrorUpdated?.Invoke(this, new(plan, true, ex.Message));
                }
                finally
                {
                    PlanRunningUpdated?.Invoke(this, new(plan, false));
                    PlanStatusTextUpdated?.Invoke(this, new(plan, "Finished successfully"));
                }
            });
        }
    }
}

