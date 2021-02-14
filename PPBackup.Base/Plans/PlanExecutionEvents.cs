using System;

namespace PPBackup.Base.Plans
{
    public class PlanProgressUpdatedEventArgs
    {
        public PlanProgressUpdatedEventArgs(BackupPlan backupPlan, int progress)
        {
            BackupPlan = backupPlan;
            Progress = progress;
        }

        public BackupPlan BackupPlan { get; }
        public int Progress { get; }
    }

    public class PlanStatusTextUpdatedEventArgs
    {
        public PlanStatusTextUpdatedEventArgs(BackupPlan backupPlan, string statusText)
        {
            BackupPlan = backupPlan;
            StatusText = statusText;
        }

        public BackupPlan BackupPlan { get; }
        public string StatusText { get; }
    }

    public class PlanCanExecuteUpdatedEventArgs
    {
        public PlanCanExecuteUpdatedEventArgs(BackupPlan backupPlan, bool canExecute)
        {
            BackupPlan = backupPlan;
            CanExecute = canExecute;
        }

        public BackupPlan BackupPlan { get; }
        public bool CanExecute { get; }
    }

    public class PlanHasErrorsUpdatedEventArgs
    {
        public PlanHasErrorsUpdatedEventArgs(BackupPlan backupPlan, bool hasErrors, string statusText = "")
        {
            BackupPlan = backupPlan;
            HasErrors = hasErrors;
            StatusText = statusText;
        }

        public BackupPlan BackupPlan { get; }
        public bool HasErrors { get; }
        public string StatusText { get; }
    }

    public class PlanIsRunningUpdatedEventArgs
    {
        public PlanIsRunningUpdatedEventArgs(BackupPlan backupPlan, bool isRunning)
        {
            BackupPlan = backupPlan;
            IsRunning = isRunning;
        }

        public BackupPlan BackupPlan { get; }
        public bool IsRunning { get; }
    }

    public interface IPlanExecutionEvents
    {
        event EventHandler<PlanProgressUpdatedEventArgs> ProgressUpdated;
        event EventHandler<PlanStatusTextUpdatedEventArgs> StatusTextUpdated;
        event EventHandler<PlanCanExecuteUpdatedEventArgs> CanExecuteUpdated;
        event EventHandler<PlanHasErrorsUpdatedEventArgs> HasErrorsUpdated;
        event EventHandler<PlanIsRunningUpdatedEventArgs> IsRunningUpdated;
    }

    public class PlanExecutionEvents : IPlanExecutionEvents
    {
        private readonly BackupPlan backupPlan;

        public PlanExecutionEvents(BackupPlan backupPlan)
        {
            this.backupPlan = backupPlan;
        }

        public event EventHandler<PlanProgressUpdatedEventArgs>? ProgressUpdated;

        public PlanExecutionEvents Progress(int progress)
        {
            ProgressUpdated?.Invoke(this, new PlanProgressUpdatedEventArgs(backupPlan, progress));
            return this;
        }

        public event EventHandler<PlanStatusTextUpdatedEventArgs>? StatusTextUpdated;

        public PlanExecutionEvents StatusText(string statusText)
        {
            StatusTextUpdated?.Invoke(this, new PlanStatusTextUpdatedEventArgs(backupPlan, statusText));
            return this;
        }

        public event EventHandler<PlanCanExecuteUpdatedEventArgs>? CanExecuteUpdated;

        public PlanExecutionEvents CanExecute(bool canExecute)
        {
            CanExecuteUpdated?.Invoke(this, new PlanCanExecuteUpdatedEventArgs(backupPlan, canExecute));
            return this;
        }

        public event EventHandler<PlanHasErrorsUpdatedEventArgs>? HasErrorsUpdated;

        public PlanExecutionEvents HasErrors(bool hasErrors, string statusText = "")
        {
            HasErrorsUpdated?.Invoke(this, new PlanHasErrorsUpdatedEventArgs(backupPlan, hasErrors, statusText));
            return this;
        }

        public event EventHandler<PlanIsRunningUpdatedEventArgs>? IsRunningUpdated;

        public PlanExecutionEvents IsRunning(bool isRunning)
        {
            IsRunningUpdated?.Invoke(this, new PlanIsRunningUpdatedEventArgs(backupPlan, isRunning));
            return this;
        }
    }
}
