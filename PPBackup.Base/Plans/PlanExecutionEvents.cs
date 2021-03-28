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
        event EventHandler<PlanProgressUpdatedEventArgs> Progress;
        event EventHandler<PlanStatusTextUpdatedEventArgs> StatusText;
        event EventHandler<PlanCanExecuteUpdatedEventArgs> CanExecute;
        event EventHandler<PlanHasErrorsUpdatedEventArgs> HasErrors;
        event EventHandler<PlanIsRunningUpdatedEventArgs> IsRunning;
    }

    public class PlanExecutionEvents : IPlanExecutionEvents
    {
        private readonly BackupPlan backupPlan;

        public PlanExecutionEvents(BackupPlan backupPlan)
        {
            this.backupPlan = backupPlan;
        }

        public event EventHandler<PlanProgressUpdatedEventArgs>? Progress;

        public PlanExecutionEvents OnProgress(int progress)
        {
            Progress?.Invoke(this, new PlanProgressUpdatedEventArgs(backupPlan, progress));
            return this;
        }

        public event EventHandler<PlanStatusTextUpdatedEventArgs>? StatusText;

        public PlanExecutionEvents OnStatusText(string statusText)
        {
            StatusText?.Invoke(this, new PlanStatusTextUpdatedEventArgs(backupPlan, statusText));
            return this;
        }

        public event EventHandler<PlanCanExecuteUpdatedEventArgs>? CanExecute;

        public PlanExecutionEvents OnCanExecute(bool canExecute)
        {
            CanExecute?.Invoke(this, new PlanCanExecuteUpdatedEventArgs(backupPlan, canExecute));
            return this;
        }

        public event EventHandler<PlanHasErrorsUpdatedEventArgs>? HasErrors;

        public PlanExecutionEvents OnHasErrors(bool hasErrors, string statusText = "")
        {
            HasErrors?.Invoke(this, new PlanHasErrorsUpdatedEventArgs(backupPlan, hasErrors, statusText));
            return this;
        }

        public event EventHandler<PlanIsRunningUpdatedEventArgs>? IsRunning;

        public PlanExecutionEvents OnIsRunning(bool isRunning)
        {
            IsRunning?.Invoke(this, new PlanIsRunningUpdatedEventArgs(backupPlan, isRunning));
            return this;
        }
    }
}
