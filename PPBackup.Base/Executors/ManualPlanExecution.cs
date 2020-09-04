using PPBackup.Base.Model;
using System.Threading.Tasks;

namespace PPBackup.Base.Executors
{
    internal class ManualPlanExecution : IPlanExecution
    {
        private readonly PlanExecutionHelper planExecutionHelper;
        private readonly BackupPlan backupPlan;
        private readonly PlanExecutionEvents events;

        public class Creator : IPlanExecutionCreator
        {
            private readonly PlanExecutionHelper planExecutionHelper;

            public Creator(PlanExecutionHelper planExecutionHelper)
            {
                this.planExecutionHelper = planExecutionHelper;
            }

            public string RunType => "manually";

            public IPlanExecution Create(BackupPlan plan, PlanExecutionEvents events) => new ManualPlanExecution(planExecutionHelper, plan, events);
        }

        public ManualPlanExecution(PlanExecutionHelper planExecutionHelper, BackupPlan backupPlan, PlanExecutionEvents events)
        {
            this.planExecutionHelper = planExecutionHelper;
            this.backupPlan = backupPlan;
            this.events = events;
        }

        public void NotifyInitialStates()
        {
            events
                .CanExecute(planExecutionHelper.CanExecute(backupPlan))
                .HasErrors(false)
                .IsRunning(false)
                .StatusText("Not run yet");
        }

        public async Task ExecuteAsync()
        {
            if (backupPlan != null && events != null)
            {
                await planExecutionHelper.ExecuteAsync(backupPlan, events);
            }
        }
    }
}
