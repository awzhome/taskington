using System;

namespace Taskington.Base.Plans;

public interface IPlanExecution
{
    event EventHandler<PlanProgressUpdatedEventArgs>? PlanProgressUpdated;
    event EventHandler<PlanStatusTextUpdatedEventArgs>? PlanStatusTextUpdated;
    event EventHandler<PlanErrorUpdatedEventArgs>? PlanErrorUpdated;
    event EventHandler<PlanCanExecuteUpdatedEventArgs>? PlanCanExecuteUpdated;
    event EventHandler<PlanRunningUpdatedEventArgs>? PlanRunningUpdated;
    event EventHandler<PlanPreCheckRequestedEventArgs>? PlanPreCheckRequested;

    void NotifyInitialStates(Plan plan);
    void Execute(Plan plan);
}

public class PlanProgressUpdatedEventArgs : EventArgs
{
    public PlanProgressUpdatedEventArgs(Plan plan, int progress)
    {
        Plan = plan;
        Progress = progress;
    }

    public Plan Plan { get; }
    public int Progress { get; }
}

public class PlanStatusTextUpdatedEventArgs : EventArgs
{
    public PlanStatusTextUpdatedEventArgs(Plan plan, string statusText)
    {
        Plan = plan;
        StatusText = statusText;
    }

    public Plan Plan { get; }
    public string StatusText { get; }
}

public class PlanErrorUpdatedEventArgs : EventArgs
{
    public PlanErrorUpdatedEventArgs(Plan plan, bool hasErrors, string? errorText)
    {
        Plan = plan;
        HasErrors = hasErrors;
        ErrorText = errorText;
    }

    public Plan Plan { get; }
    public bool HasErrors { get; }
    public string? ErrorText { get; }

}

public class PlanCanExecuteUpdatedEventArgs : EventArgs
{
    public PlanCanExecuteUpdatedEventArgs(Plan plan, bool canExecute)
    {
        Plan = plan;
        CanExecute = canExecute;
    }

    public Plan Plan { get; }
    public bool CanExecute { get; }
}

public class PlanRunningUpdatedEventArgs : EventArgs
{
    public PlanRunningUpdatedEventArgs(Plan plan, bool running)
    {
        Plan = plan;
        Running = running;
    }

    public Plan Plan { get; }
    public bool Running { get; }
}

public class PlanPreCheckRequestedEventArgs : EventArgs
{
    private bool canExecute = true;

    public PlanPreCheckRequestedEventArgs(Plan plan)
    {
        Plan = plan;
    }

    public Plan Plan { get; }

    public bool CanExecute
    {
        get => canExecute;
        set => canExecute &= value;
    }
}