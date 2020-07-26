using PPBackup.Base.Model;
using System.Threading.Tasks;

namespace PPBackup.Base.Executors
{
    internal class ManualPlanExecution : IPlanExecution
    {
        private readonly PlanExecutionHelper planExecutionHelper;

        public BackupPlan BackupPlan { get; }

        public PlanExecutionStatus Status { get; }

        public class Creator : IPlanExecutionCreator
        {
            private readonly PlanExecutionHelper planExecutionHelper;

            public Creator(PlanExecutionHelper planExecutionHelper)
            {
                this.planExecutionHelper = planExecutionHelper;
            }

            public string RunType => "manually";

            public IPlanExecution Create(BackupPlan plan, PlanExecutionStatus status) => new ManualPlanExecution(planExecutionHelper, plan, status);
        }

        public ManualPlanExecution(PlanExecutionHelper planExecutionHelper, BackupPlan plan, PlanExecutionStatus status)
        {
            this.planExecutionHelper = planExecutionHelper;
            BackupPlan = plan;
            Status = status;
        }

        public async Task ExecuteAsync()
        {
            if (BackupPlan != null && Status != null)
            {
                await planExecutionHelper.ExecuteAsync(BackupPlan, Status);
            }
        }
    }
}
