using System.Threading.Tasks;
using Taskington.Base.Events;

namespace Taskington.Base.Plans
{
    internal class ManualPlanExecution : IPlanExecution
    {
        private readonly ApplicationEvents events;
        private readonly PlanExecutionHelper planExecutionHelper;
        private readonly Plan plan;

        public class Creator : IPlanExecutionCreator
        {
            private readonly ApplicationEvents events;
            private readonly PlanExecutionHelper planExecutionHelper;

            public Creator(ApplicationEvents events, PlanExecutionHelper planExecutionHelper)
            {
                this.events = events;
                this.planExecutionHelper = planExecutionHelper;
            }

            public string RunType => Plan.OnSelectionRunType;

            public IPlanExecution Create(Plan plan) => new ManualPlanExecution(events, planExecutionHelper, plan);
        }

        public ManualPlanExecution(ApplicationEvents events, PlanExecutionHelper planExecutionHelper, Plan plan)
        {
            this.events = events;
            this.planExecutionHelper = planExecutionHelper;
            this.plan = plan;
        }

        public void NotifyInitialStates()
        {
            events
                .OnPlanCanExecute(plan, planExecutionHelper.CanExecute(plan))
                .OnPlanHasErrors(plan, false)
                .OnPlanIsRunning(plan, false)
                .OnPlanStatusText(plan, "Not run yet");
        }

        public async Task ExecuteAsync()
        {
            if (plan != null)
            {
                await planExecutionHelper.ExecuteAsync(plan);
            }
        }
    }
}
