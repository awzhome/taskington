using Taskington.Base.Events;
using Taskington.Base.TinyBus;

namespace Taskington.Base.Plans
{
    class InvalidPlanExecution
    {
        private readonly IEventBus eventBus;

        public InvalidPlanExecution(IEventBus eventBus)
        {
            this.eventBus = eventBus;

            eventBus
                .Subscribe<NotifyInitialPlanStates>(NotifyInitialStates);
        }
        private void NotifyInitialStates(NotifyInitialPlanStates e)
        {
            if (!e.Plan.IsValid)
            {
                eventBus
                    .Push(new PlanCanExecuteUpdated(e.Plan, false))
                    .Push(new PlanIsRunningUpdated(e.Plan, false))
                    .Push(new PlanHasErrorsUpdated(e.Plan, true, e.Plan.ValidationMessage ?? ""));
            }
        }
    }
}
