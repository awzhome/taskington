namespace Taskington.Base.Plans
{
    class InvalidPlanExecution
    {
        public InvalidPlanExecution()
        {
            NotifyInitialPlanStatesMessage.Subscribe(m => NotifyInitialStates(m.Plan));
        }

        private void NotifyInitialStates(Plan plan)
        {
            if (!plan.IsValid)
            {
                new PlanCanExecuteUpdateMessage(plan, false).Publish();
                new PlanRunningUpdateMessage(plan, false).Publish();
                new PlanErrorUpdateMessage(plan, true, plan.ValidationMessage).Publish();
            }
        }
    }
}
