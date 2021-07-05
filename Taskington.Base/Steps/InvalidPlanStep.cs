namespace Taskington.Base.Steps
{
    public class InvalidPlanStep : PlanStep
    {
        public InvalidPlanStep(string errorMessage) : base("invalid")
        {
            ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; }
    }
}
