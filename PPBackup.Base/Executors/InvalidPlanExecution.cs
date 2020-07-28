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

        public Task ExecuteAsync()
        {
            events.HasErrors(true, reason);
            return Task.CompletedTask;
        }
    }
}
