using System;
using Taskington.Base.Steps;
using Taskington.Base.SystemOperations;
using Taskington.Base.TinyBus;

namespace Taskington.Base.Plans
{
    public static class PlanEvents
    {
        public static Event<int> StepProgressUpdated { get; } = new();
        public static Event<string> StepStatusTextUpdated { get; } = new();
        public static Event<Plan, int> PlanProgressUpdated { get; } = new();
        public static Event<Plan, string> PlanStatusTextUpdated { get; } = new();
        public static Event<Plan, bool> PlanCanExecuteUpdated { get; } = new();
        public static Event<Plan, bool, string?> PlanHasErrorsUpdated { get; } = new();
        public static Event<Plan, bool> PlanIsRunningUpdated { get; } = new();
        public static Event<Plan> ExecutePlan { get; } = new();
        public static Event<Plan> NotifyInitialPlanStates { get; } = new();
        public static Event<Plan> PreCheckPlanExecution { get; } = new();
        public static Event<PlanStep, Placeholders, Action<int>, Action<string>> ExecuteStep { get; } = new();

    }
}
