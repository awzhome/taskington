using System;

namespace Taskington.Base.Plans
{
    public class PlanProgressUpdatedEventArgs
    {
        public PlanProgressUpdatedEventArgs(Plan plan, int progress)
        {
            Plan = plan;
            Progress = progress;
        }

        public Plan Plan { get; }
        public int Progress { get; }
    }

    public class PlanStatusTextUpdatedEventArgs
    {
        public PlanStatusTextUpdatedEventArgs(Plan plan, string statusText)
        {
            Plan = plan;
            StatusText = statusText;
        }

        public Plan Plan { get; }
        public string StatusText { get; }
    }

    public class PlanCanExecuteUpdatedEventArgs
    {
        public PlanCanExecuteUpdatedEventArgs(Plan plan, bool canExecute)
        {
            Plan = plan;
            CanExecute = canExecute;
        }

        public Plan Plan { get; }
        public bool CanExecute { get; }
    }

    public class PlanHasErrorsUpdatedEventArgs
    {
        public PlanHasErrorsUpdatedEventArgs(Plan plan, bool hasErrors, string statusText = "")
        {
            Plan = plan;
            HasErrors = hasErrors;
            StatusText = statusText;
        }

        public Plan Plan { get; }
        public bool HasErrors { get; }
        public string StatusText { get; }
    }

    public class PlanIsRunningUpdatedEventArgs
    {
        public PlanIsRunningUpdatedEventArgs(Plan plan, bool isRunning)
        {
            Plan = plan;
            IsRunning = isRunning;
        }

        public Plan Plan { get; }
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
        private readonly Plan plan;

        public PlanExecutionEvents(Plan plan)
        {
            this.plan = plan;
        }

        public event EventHandler<PlanProgressUpdatedEventArgs>? Progress;

        public PlanExecutionEvents OnProgress(int progress)
        {
            Progress?.Invoke(this, new PlanProgressUpdatedEventArgs(plan, progress));
            return this;
        }

        public event EventHandler<PlanStatusTextUpdatedEventArgs>? StatusText;

        public PlanExecutionEvents OnStatusText(string statusText)
        {
            StatusText?.Invoke(this, new PlanStatusTextUpdatedEventArgs(plan, statusText));
            return this;
        }

        public event EventHandler<PlanCanExecuteUpdatedEventArgs>? CanExecute;

        public PlanExecutionEvents OnCanExecute(bool canExecute)
        {
            CanExecute?.Invoke(this, new PlanCanExecuteUpdatedEventArgs(plan, canExecute));
            return this;
        }

        public event EventHandler<PlanHasErrorsUpdatedEventArgs>? HasErrors;

        public PlanExecutionEvents OnHasErrors(bool hasErrors, string statusText = "")
        {
            HasErrors?.Invoke(this, new PlanHasErrorsUpdatedEventArgs(plan, hasErrors, statusText));
            return this;
        }

        public event EventHandler<PlanIsRunningUpdatedEventArgs>? IsRunning;

        public PlanExecutionEvents OnIsRunning(bool isRunning)
        {
            IsRunning?.Invoke(this, new PlanIsRunningUpdatedEventArgs(plan, isRunning));
            return this;
        }
    }
}
