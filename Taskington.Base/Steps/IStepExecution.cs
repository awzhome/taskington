using System;
using Taskington.Base.SystemOperations;

namespace Taskington.Base.Steps;

public interface IStepExecution
{
    void Execute(PlanStep step, Placeholders placeholders, Action<int> progressCallback, Action<string> statusTextCallback);
}