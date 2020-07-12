using PPBackup.Base.Model;
using PPBackup.Base.SystemOperations;

namespace PPBackup.Base.Executors
{
    public interface IStepExecution
    {
        string Type { get; }
        void Execute(BackupStep backupStep, Placeholders placeholders, StepExecutionStatus status);
    }
}
