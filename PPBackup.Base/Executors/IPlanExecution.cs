using PPBackup.Base.Model;

namespace PPBackup.Base.Executors
{
    public interface IPlanExecutionCreator
    {
        string RunType { get; }
        IPlanExecution Create(BackupPlan plan, PlanExecutionStatus status);
    }

    public interface IPlanExecution
    {
        void Execute();
        BackupPlan BackupPlan { get; }
        PlanExecutionStatus Status { get; }
    }
}
