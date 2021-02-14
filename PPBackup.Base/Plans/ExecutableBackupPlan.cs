namespace PPBackup.Base.Plans
{
    public class ExecutableBackupPlan
    {
        public ExecutableBackupPlan(BackupPlan backupPlan, IPlanExecution execution, IPlanExecutionEvents events)
        {
            BackupPlan = backupPlan;
            Execution = execution;
            Events = events;
        }

        public BackupPlan BackupPlan { get; }
        public IPlanExecution Execution { get; }
        public IPlanExecutionEvents Events { get; }
    }
}
