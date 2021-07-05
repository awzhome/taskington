using System.Threading.Tasks;

namespace Taskington.Base.Plans
{
    internal class ManualPlanExecution : IPlanExecution
    {
        private readonly PlanExecutionHelper planExecutionHelper;
        private readonly Plan plan;
        private readonly PlanExecutionEvents events;

        public class Creator : IPlanExecutionCreator
        {
            private readonly PlanExecutionHelper planExecutionHelper;

            public Creator(PlanExecutionHelper planExecutionHelper)
            {
                this.planExecutionHelper = planExecutionHelper;
            }

            public string RunType => Plan.OnSelectionRunType;

            public IPlanExecution Create(Plan plan, PlanExecutionEvents events) => new ManualPlanExecution(planExecutionHelper, plan, events);
        }

        public ManualPlanExecution(PlanExecutionHelper planExecutionHelper, Plan plan, PlanExecutionEvents events)
        {
            this.planExecutionHelper = planExecutionHelper;
            this.plan = plan;
            this.events = events;
        }

        public void NotifyInitialStates()
        {
            events
                .OnCanExecute(planExecutionHelper.CanExecute(plan))
                .OnHasErrors(false)
                .OnIsRunning(false)
                .OnStatusText("Not run yet");
        }

        public async Task ExecuteAsync()
        {
            if (plan != null && events != null)
            {
                await planExecutionHelper.ExecuteAsync(plan, events);
            }
        }
    }
}
