using System.Collections.Generic;
using Taskington.Base.SystemOperations;

namespace Taskington.Base.Steps
{
    public interface IStepExecution
    {
        string Type { get; }
        void Execute(PlanStep step, Placeholders placeholders, StepExecutionEvents status);
        bool CanExecuteSupportedSteps(IEnumerable<PlanStep> steps, Placeholders placeholders);
    }
}
