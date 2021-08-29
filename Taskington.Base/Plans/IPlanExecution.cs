using System.Threading.Tasks;

namespace Taskington.Base.Plans
{
    public interface IPlanExecutionCreator
    {
        string RunType { get; }
        IPlanExecution Create(Plan plan);
    }

    public interface IPlanExecution
    {
        void NotifyInitialStates();
        Task Execute();
    }
}
