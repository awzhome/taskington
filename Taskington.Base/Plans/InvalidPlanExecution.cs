using System;

namespace Taskington.Base.Plans
{
    class InvalidPlanExecution : IPlanExecution
    {
        public event EventHandler<PlanProgressUpdatedEventArgs>? PlanProgressUpdated;
        public event EventHandler<PlanStatusTextUpdatedEventArgs>? PlanStatusTextUpdated;
        public event EventHandler<PlanErrorUpdatedEventArgs>? PlanErrorUpdated;
        public event EventHandler<PlanCanExecuteUpdatedEventArgs>? PlanCanExecuteUpdated;
        public event EventHandler<PlanRunningUpdatedEventArgs>? PlanRunningUpdated;
        public event EventHandler<PlanPreCheckRequestedEventArgs>? PlanPreCheckRequested;

        public void Execute(Plan plan)
        {
            // No-op
        }

        public void NotifyInitialStates(Plan plan)
        {
            if (!plan.IsValid)
            {
                PlanCanExecuteUpdated?.Invoke(this, new(plan, false));
                PlanRunningUpdated?.Invoke(this, new(plan, false));
                PlanErrorUpdated?.Invoke(this, new(plan, true, plan.ValidationMessage));
            }
        }
    }
}
