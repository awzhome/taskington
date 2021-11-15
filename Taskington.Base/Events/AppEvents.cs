using System;
using Taskington.Base.Plans;
using Taskington.Base.Steps;
using Taskington.Base.SystemOperations;
using Taskington.Base.TinyBus;

namespace Taskington.Base.Events
{
    public record ConfigurationChanged() : IEvent;
    public record ConfigurationReloaded() : IEvent;
    public record ConfigurationReloadDelayed(bool IsDelayed) : IEvent;
    public record StepProgressUpdated(int Progress) : IEvent;
    public record StepStatusTextUpdated(string StatusText) : IEvent;
    public record PlanProgressUpdated(Plan Plan, int Progress) : IEvent;
    public record PlanStatusTextUpdated(Plan Plan, string StatusText) : IEvent;
    public record PlanCanExecuteUpdated(Plan Plan, bool CanExecute) : IEvent;
    public record PlanHasErrorsUpdated(Plan Plan, bool HasErrors, string StatusText = "") : IEvent;
    public record PlanIsRunningUpdated(Plan Plan, bool IsRunning) : IEvent;
    public record ExecutePlan(Plan Plan) : IEvent;
    public record NotifyInitialPlanStates(Plan Plan) : IEvent;
    public record ExecuteStep(PlanStep Step, Placeholders Placeholders, Action<int>? ProgressCallback, Action<string>? StatusTextCallback) : IEvent;

    public record SyncDirectory(SyncDirection SyncDirection, string FromDir, string ToDir) : IEvent;
    public record SyncFile(string FromDir, string ToDir, string File) : IEvent;
    public record LoadSystemPlaceholders() : IResponse<Placeholders>;

    public record InitializeConfiguration() : IEvent;
    public record GetConfigValue(string Key) : IResponse<string?>;
    public record SetConfigValue(string Key, string Value) : IEvent;
    public record SaveConfiguration() : IEvent;
    public record InsertPlan(int Index, Plan NewPlan): IEvent;
    public record RemovePlan(Plan Plan): IEvent;
    public record ReplacePlan(Plan OldPlan, Plan NewPlan): IEvent;
}
