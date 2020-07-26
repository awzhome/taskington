using PPBackup.Base.Model;
using System.Threading.Tasks;

namespace PPBackup.Base.Executors
{
    public interface IPlanExecutionCreator
    {
        string RunType { get; }
        IPlanExecution Create(BackupPlan plan, PlanExecutionStatus status);
    }

    public interface IPlanExecution
    {
        Task ExecuteAsync();
        BackupPlan BackupPlan { get; }
        PlanExecutionStatus Status { get; }
    }
}
