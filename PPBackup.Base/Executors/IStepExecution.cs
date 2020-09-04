using PPBackup.Base.Model;
using PPBackup.Base.SystemOperations;
using System.Collections.Generic;

namespace PPBackup.Base.Executors
{
    public interface IStepExecution
    {
        string Type { get; }
        void Execute(BackupStep backupStep, Placeholders placeholders, StepExecutionEvents status);
        bool CanExecuteSupportedSteps(IEnumerable<BackupStep> backupSteps, Placeholders placeholders);
    }
}
