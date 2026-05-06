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

public class PlanProgressUpdatedEventArgs(Plan plan, int progress) : EventArgs
{
    public Plan Plan { get; } = plan;
    public int Progress { get; } = progress;
}

public class PlanStatusTextUpdatedEventArgs(Plan plan, string statusText) : EventArgs
{
    public Plan Plan { get; } = plan;
    public string StatusText { get; } = statusText;
}

public class PlanErrorUpdatedEventArgs(Plan plan, bool hasErrors, string? errorText) : EventArgs
{
    public Plan Plan { get; } = plan;
    public bool HasErrors { get; } = hasErrors;
    public string? ErrorText { get; } = errorText;
}

public class PlanCanExecuteUpdatedEventArgs(Plan plan, bool canExecute) : EventArgs
{
    public Plan Plan { get; } = plan;
    public bool CanExecute { get; } = canExecute;
}

public class PlanRunningUpdatedEventArgs(Plan plan, bool running) : EventArgs
{
    public Plan Plan { get; } = plan;
    public bool Running { get; } = running;
}

public class PlanPreCheckRequestedEventArgs(Plan plan) : EventArgs
{
    private bool canExecute = true;

    public Plan Plan { get; } = plan;

    public bool CanExecute
    {
        get => canExecute;
        set => canExecute &= value;
    }
}