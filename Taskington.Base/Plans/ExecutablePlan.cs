namespace Taskington.Base.Plans
{
    public class ExecutablePlan
    {
        public ExecutablePlan(Plan plan, IPlanExecution execution, IPlanExecutionEvents events)
        {
            Plan = plan;
            Execution = execution;
            Events = events;
        }

        public Plan Plan { get; }
        public IPlanExecution Execution { get; }
        public IPlanExecutionEvents Events { get; }
    }
}
