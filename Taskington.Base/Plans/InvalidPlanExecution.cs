using System.Threading.Tasks;
using Taskington.Base.Events;

namespace Taskington.Base.Plans
{
    class InvalidPlanExecution : IPlanExecution
    {
        private readonly Plan plan;
        private readonly ApplicationEvents events;
        private readonly string reason;

        public InvalidPlanExecution(Plan plan, ApplicationEvents events, string reason)
        {
            this.plan = plan;
            this.events = events;
            this.reason = reason;
        }
        public void NotifyInitialStates()
        {
            events
                .OnPlanCanExecute(plan,false)
                .OnPlanIsRunning(plan, false)
                .OnPlanHasErrors(plan, true, reason);
        }

        public Task Execute()
        {
            return Task.CompletedTask;
        }
    }
}
