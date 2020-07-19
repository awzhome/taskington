using PPBackup.Base.Model;
using System;

namespace PPBackup.Base.Executors
{
    class InvalidPlanExecution : IPlanExecution
    {
        public BackupPlan BackupPlan { get; }

        public PlanExecutionStatus Status { get; }

        public InvalidPlanExecution(BackupPlan backupPlan, PlanExecutionStatus status)
        {
            BackupPlan = backupPlan;
            Status = status;
        }

        public void Execute()
        {
            Status.NotifyPropertyChange(nameof(Status.HasErrors));
        }
    }
}
