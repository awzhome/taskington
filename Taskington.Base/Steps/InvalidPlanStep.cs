namespace Taskington.Base.Steps;

public class InvalidPlanStep(string errorMessage) : PlanStep("invalid")
{
    public string ErrorMessage { get; } = errorMessage;
}