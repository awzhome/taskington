using System;
using Taskington.Base.Steps;
using Taskington.Base.SystemOperations;
using Taskington.Base.TinyBus.Endpoints;

namespace Taskington.Base.Plans
{
    public static class PlanMessages
    {
        public static MessageEndPoint<int> StepProgressUpdated { get; } = new();
        public static MessageEndPoint<string> StepStatusTextUpdated { get; } = new();
        public static MessageEndPoint<Plan, int> PlanProgressUpdated { get; } = new();
        public static MessageEndPoint<Plan, string> PlanStatusTextUpdated { get; } = new();
        public static MessageEndPoint<Plan, bool> PlanCanExecuteUpdated { get; } = new();
        public static MessageEndPoint<Plan, bool, string?> PlanHasErrorsUpdated { get; } = new();
        public static MessageEndPoint<Plan, bool> PlanIsRunningUpdated { get; } = new();
        public static MessageEndPoint<Plan> ExecutePlan { get; } = new();
        public static MessageEndPoint<Plan> NotifyInitialPlanStates { get; } = new();
        public static MessageEndPoint<Plan> PreCheckPlanExecution { get; } = new();
        public static MessageEndPoint<PlanStep, Placeholders, Action<int>, Action<string>> ExecuteStep { get; } = new();

    }
}
