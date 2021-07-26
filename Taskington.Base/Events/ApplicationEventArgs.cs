using System;
using Taskington.Base.Plans;

namespace Taskington.Base.Events
{
    public class ConfigurationReloadDelayEventArgs : EventArgs
    {
        public bool IsDelayed { get; set; }
    }

    public class StepProgressUpdatedEventArgs
    {
        public StepProgressUpdatedEventArgs(int progress)
        {
            Progress = progress;
        }

        public int Progress { get; }
    }

    public class StepStatusTextUpdatedEventArgs
    {
        public StepStatusTextUpdatedEventArgs(string statusText)
        {
            StatusText = statusText;
        }

        public string StatusText { get; }
    }

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
}
