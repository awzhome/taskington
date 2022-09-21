using System;
using Taskington.Base.Steps;
using Taskington.Base.SystemOperations;
using Taskington.Base.TinyBus;
using Taskington.Base.TinyBus.Endpoints;

namespace Taskington.Base.Plans;

public record PlanProgressUpdateMessage(Plan Plan, int Progress) : Message<PlanProgressUpdateMessage>;
public record PlanStatusTextUpdateMessage(Plan Plan, string StatusText) : Message<PlanStatusTextUpdateMessage>;
public record PlanErrorUpdateMessage(Plan Plan, bool HasErrors, string? ValidationText) : Message<PlanErrorUpdateMessage>;
public record PlanRunningUpdateMessage(Plan Plan, bool IsRunning) : Message<PlanRunningUpdateMessage>;
public record PlanCanExecuteUpdateMessage(Plan Plan, bool CanExecute) : Message<PlanCanExecuteUpdateMessage>;
public record ExecutePlanMessage(Plan Plan) : Message<ExecutePlanMessage>;
public record NotifyInitialPlanStatesMessage(Plan Plan) : Message<NotifyInitialPlanStatesMessage>;
public record PreCheckPlanExecutionMessage(Plan Plan) : Message<PreCheckPlanExecutionMessage>;
public record ExecuteStepMessage(
        PlanStep Step,
        Placeholders Placeholders,
        Action<int> ProgressCallback,
        Action<string> StatusTextCallback
    ) : Message<ExecuteStepMessage>;

