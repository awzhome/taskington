using System.Threading.Tasks;

namespace PPBackup.Base.Plans
{
    public interface IPlanExecutionCreator
    {
        string RunType { get; }
        IPlanExecution Create(BackupPlan plan, PlanExecutionEvents events);
    }

    public interface IPlanExecution
    {
        void NotifyInitialStates();
        Task ExecuteAsync();
    }
}
