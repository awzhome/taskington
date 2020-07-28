using PPBackup.Base.Model;
using System.Threading.Tasks;

namespace PPBackup.Base.Executors
{
    public interface IPlanExecutionCreator
    {
        string RunType { get; }
        IPlanExecution Create(BackupPlan plan, PlanExecutionEvents events);
    }

    public interface IPlanExecution
    {
        Task ExecuteAsync();
    }
}
