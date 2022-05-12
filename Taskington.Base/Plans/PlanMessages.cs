using System;
using Taskington.Base.Steps;
using Taskington.Base.SystemOperations;
using Taskington.Base.TinyBus;

namespace Taskington.Base.Plans
{
    public static class PlanMessages
    {
        public static Message<int> StepProgressUpdated { get; } = new();
        public static Message<string> StepStatusTextUpdated { get; } = new();
        public static Message<Plan, int> PlanProgressUpdated { get; } = new();
        public static Message<Plan, string> PlanStatusTextUpdated { get; } = new();
        public static Message<Plan, bool> PlanCanExecuteUpdated { get; } = new();
        public static Message<Plan, bool, string?> PlanHasErrorsUpdated { get; } = new();
        public static Message<Plan, bool> PlanIsRunningUpdated { get; } = new();
        public static Message<Plan> ExecutePlan { get; } = new();
        public static Message<Plan> NotifyInitialPlanStates { get; } = new();
        public static Message<Plan> PreCheckPlanExecution { get; } = new();
        public static Message<PlanStep, Placeholders, Action<int>, Action<string>> ExecuteStep { get; } = new();

    }
}
