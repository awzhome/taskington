using PPBackup.Base.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PPBackup.Base.Executors
{
    public class ExecutableBackupPlan
    {
        public ExecutableBackupPlan(BackupPlan plan, IPlanExecution planExecution, PlanExecutionStatus status)
        {
            Plan = plan;
            PlanExecution = planExecution;
            Status = status;
        }

        public BackupPlan Plan { get; }
        public IPlanExecution PlanExecution { get; }
        public PlanExecutionStatus Status { get; }
    }
}
