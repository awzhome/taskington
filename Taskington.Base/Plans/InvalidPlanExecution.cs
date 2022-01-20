namespace Taskington.Base.Plans
{
    class InvalidPlanExecution
    {
        public InvalidPlanExecution()
        {
            PlanEvents.NotifyInitialPlanStates.Subscribe(NotifyInitialStates);
        }

        private void NotifyInitialStates(Plan plan)
        {
            if (!plan.IsValid)
            {
                PlanEvents.PlanCanExecuteUpdated.Push(plan, false);
                PlanEvents.PlanIsRunningUpdated.Push(plan, false);
                PlanEvents.PlanHasErrorsUpdated.Push(plan, true, plan.ValidationMessage);
            }
        }
    }
}
