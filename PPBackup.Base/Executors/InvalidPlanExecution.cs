using PPBackup.Base.Model;
using System;
using System.Threading.Tasks;

namespace PPBackup.Base.Executors
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
                .CanExecute(false)
                .IsRunning(false)
                .HasErrors(true, reason);
        }

        public Task ExecuteAsync()
        {
            return Task.CompletedTask;
        }
    }
}
