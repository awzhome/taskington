using System.Collections.Generic;
using Taskington.Base.SystemOperations;

namespace Taskington.Base.Steps
{
    public interface IStepExecution
    {
        string Type { get; }
        void Execute(BackupStep backupStep, Placeholders placeholders, StepExecutionEvents status);
        bool CanExecuteSupportedSteps(IEnumerable<BackupStep> backupSteps, Placeholders placeholders);
    }
}
