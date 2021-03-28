using System.Threading.Tasks;

namespace PPBackup.Base.Plans
{
    class InvalidPlanExecution : IPlanExecution
    {
        private readonly PlanExecutionEvents events;
        private readonly string reason;

        public InvalidPlanExecution(PlanExecutionEvents events, string reason)
        {
            this.events = events;
            this.reason = reason;
        }
        public void NotifyInitialStates()
        {
            events
                .OnCanExecute(false)
                .OnIsRunning(false)
                .OnHasErrors(true, reason);
        }

        public Task ExecuteAsync()
        {
            return Task.CompletedTask;
        }
    }
}
