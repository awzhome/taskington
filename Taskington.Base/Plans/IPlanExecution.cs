using System.Threading.Tasks;

namespace Taskington.Base.Plans
{
    public interface IPlanExecutionCreator
    {
        string RunType { get; }
        IPlanExecution Create(Plan plan, PlanExecutionEvents events);
    }

    public interface IPlanExecution
    {
        void NotifyInitialStates();
        Task ExecuteAsync();
    }
}
