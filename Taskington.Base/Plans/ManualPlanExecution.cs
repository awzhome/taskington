using System.Threading.Tasks;

namespace PPBackup.Base.Plans
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

            public string RunType => BackupPlan.OnSelectionRunType;

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
                .OnCanExecute(planExecutionHelper.CanExecute(backupPlan))
                .OnHasErrors(false)
                .OnIsRunning(false)
                .OnStatusText("Not run yet");
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
