using PPBackup.Base;
using PPBackup.Base.Model;
using PPBackup.Base.SystemOperations;
using System.Threading.Tasks;

namespace PPBackup.Base.Executors
{
    public class PlanExecutionHelper
    {
        private readonly Application application;
        private readonly ISystemOperations systemOperations;

        public PlanExecutionHelper(Application application, ISystemOperations systemOperations)
        {
            this.application = application;
            this.systemOperations = systemOperations;
        }

        public async Task ExecuteAsync(BackupPlan plan, PlanExecutionStatus status)
        {
            await Task.Run(() =>
            {
                var placeholders = new Placeholders();
                systemOperations.LoadSystemPlaceholders(placeholders);

                int stepsFinished = 0;
                status.Progress = 0;
                int planProgress = 0;
                foreach (var step in plan.Steps)
                {
                    var stepExecution = application.Services.Get<IStepExecution>(s => s.Type == step.RunType);
                    if (stepExecution != null)
                    {
                        var stepStatus = new StepExecutionStatus();
                        stepStatus.PropertyChanged += (sender, e) =>
                        {
                            switch (e.PropertyName)
                            {
                                case nameof(stepStatus.StateText):
                                    status.StateText = stepStatus.StateText;
                                    break;

                                case nameof(stepStatus.Progress):
                                    status.Progress = planProgress + stepStatus.Progress / plan.Steps.Count;
                                    break;

                                default:
                                    break;
                            }
                        };
                        stepExecution.Execute(step, placeholders, stepStatus);
                        stepsFinished++;
                        planProgress = stepsFinished * 100 / plan.Steps.Count;
                        status.Progress = planProgress;
                    }
                    else
                    {
                        status.HasErrors = true;
                        status.StateText = $"Unknown execution step '{step.RunType}'";
                        break;
                    }
                }
            });
        }
    }
}
