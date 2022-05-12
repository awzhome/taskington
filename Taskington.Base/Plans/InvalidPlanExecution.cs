namespace Taskington.Base.Plans
{
    class InvalidPlanExecution
    {
        public InvalidPlanExecution()
        {
            PlanMessages.NotifyInitialPlanStates.Subscribe(NotifyInitialStates);
        }

        private void NotifyInitialStates(Plan plan)
        {
            if (!plan.IsValid)
            {
                PlanMessages.PlanCanExecuteUpdated.Push(plan, false);
                PlanMessages.PlanIsRunningUpdated.Push(plan, false);
                PlanMessages.PlanHasErrorsUpdated.Push(plan, true, plan.ValidationMessage);
            }
        }
    }
}
