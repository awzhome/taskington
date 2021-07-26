namespace Taskington.Base.Plans
{
    public class ExecutablePlan
    {
        public ExecutablePlan(Plan plan, IPlanExecution execution)
        {
            Plan = plan;
            Execution = execution;
        }

        public Plan Plan { get; }
        public IPlanExecution Execution { get; }
    }
}
