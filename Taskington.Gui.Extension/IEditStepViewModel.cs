using Taskington.Base.Steps;

namespace Taskington.Gui.Extension
{
    public interface IEditStepViewModel
    {
        public string? Icon { get; set; }

        public string? Caption { get; set; }

        public string? StepType { get; set; }

        public string? SubType { get; set; }

        PlanStep ConvertToStep();
    }
}
