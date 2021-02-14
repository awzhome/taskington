using System;

namespace PPBackup.Base.Steps
{
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

    public interface IStepExecutionEvents
    {
        event EventHandler<StepProgressUpdatedEventArgs> ProgressUpdated;
        event EventHandler<StepStatusTextUpdatedEventArgs> StatusTextUpdated;
    }

    public class StepExecutionEvents : IStepExecutionEvents
    {
        public event EventHandler<StepProgressUpdatedEventArgs>? ProgressUpdated;

        public StepExecutionEvents Progress(int progress)
        {
            ProgressUpdated?.Invoke(this, new StepProgressUpdatedEventArgs(progress));
            return this;
        }

        public event EventHandler<StepStatusTextUpdatedEventArgs>? StatusTextUpdated;

        public StepExecutionEvents StatusText(string statusText)
        {
            StatusTextUpdated?.Invoke(this, new StepStatusTextUpdatedEventArgs(statusText));
            return this;
        }
    }
}
