using ReactiveUI;
using System.Linq;
using System.Text;
using Taskington.Base.Steps;

namespace Taskington.Gui.ViewModels
{
    class EditStepViewModelBase : ViewModelBase
    {
        private readonly PlanStep step;

        public EditStepViewModelBase(PlanStep step)
        {
            this.step = step;
            InitializeFromBasicModel();
        }

        private string? icon;
        public string? Icon
        {
            get => icon;
            set => this.RaiseAndSetIfChanged(ref icon, value);
        }

        private string? caption;
        public string? Caption
        {
            get => caption;
            set => this.RaiseAndSetIfChanged(ref caption, value);
        }

        private string? stepType;
        public string? StepType
        {
            get => stepType;
            set => this.RaiseAndSetIfChanged(ref stepType, value);
        }

        private string? subType;
        public string? SubType
        {
            get => subType;
            set => this.RaiseAndSetIfChanged(ref subType, value);
        }

        private void InitializeFromBasicModel()
        {
            stepType = step.StepType;
            SubType = step.DefaultProperty;
        }

        public virtual PlanStep ConvertToStep()
        {
            return new(stepType!, step.Properties)
            {
                DefaultProperty = subType
            };
        }
    }
}
